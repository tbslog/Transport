﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Model.ReportModel;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.CurrencyExchange;

namespace TBSLogistics.Service.Services.Report
{
	public class ReportService : IReport
	{
		private readonly TMSContext _context;
		private readonly ICurrencyExchange _currencyExchange;
		private readonly ICommon _common;

		public ReportService(TMSContext context, ICommon common, ICurrencyExchange currencyExchange)
		{
			_currencyExchange = currencyExchange;
			_context = context;
			_common = common;
		}

		public async Task<GetDataReport> GetReportTransportByMonth(DateTime dateTime)
		{
			var getAllDaysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
			.Select(day => new DateTime(dateTime.Year, dateTime.Month, day)).ToList();

			var firstDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
			var lastDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

			var getTransport = await _context.VanDon.Where(x =>
			x.CreatedTime.Date >= firstDateOfMonth.Date
			&& x.CreatedTime.Date <= lastDateOfMonth.Date && x.TrangThai == 22)
				.GroupBy(x => new { x.CreatedTime.Date })
				.Select(g => new { g.Key, Count = g.Count() }).ToListAsync();
			var arrDataTransport = getTransport.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.Key.Date)).Select(x => new arrInt()
			{
				count = x.Count,
				date = x.Key.Date,
			}).ToList();
			arrDataTransport = arrDataTransport.Concat(getAllDaysInMonth.Where(x => !arrDataTransport.Select(y => y.date).Contains(x.Date)).Select(x => new arrInt()
			{
				count = 0,
				date = x.Date,
			})).ToList();

			var getHandling = await _context.DieuPhoi.Where(x => x.CreatedTime.Date >= firstDateOfMonth
			&& x.CreatedTime.Date <= lastDateOfMonth
			&& x.TrangThai == 20)
			.GroupBy(x => new { x.CreatedTime.Date })
			.Select(x => new { x.Key, Count = x.Count() }).ToListAsync();
			var arrDataHandling = getHandling.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.Key.Date)).Select(x => new arrInt()
			{
				count = x.Count,
				date = x.Key.Date,
			}).ToList();
			arrDataHandling = arrDataHandling.Concat(getAllDaysInMonth.Where(x => !arrDataHandling.Select(y => y.date).Contains(x.Date)).Select(x => new arrInt()
			{
				count = 0,
				date = x.Date,
			})).ToList();

			var arrHandling = new DataReport()
			{
				Name = "Số lượng chuyến",
				Color = "rgb(53, 162, 235)",
				DataInt = arrDataHandling
			};

			var arrTransport = new DataReport()
			{
				Name = "Số Lượng Vận Đơn",
				Color = "rgb(255, 99, 132)",
				DataInt = arrDataTransport,
			};

			var listTotal = new List<TotalReport>();
			listTotal.Add(new TotalReport { Title = "Tổng Vận Đơn", TotalInt = arrTransport.DataInt.Sum(x => x.count) });
			listTotal.Add(new TotalReport { Title = "Tổng Số Chuyến", TotalInt = arrHandling.DataInt.Sum(x => x.count) });

			return new GetDataReport()
			{
				Title = "Số Lượng Vận Đơn Và Chuyến Đã Hoàn Thành",
				TotalReports = listTotal,
				Labels = getAllDaysInMonth.Select(x => x.Date).ToList(),
				Data = new List<DataReport> { arrHandling, arrTransport },
			};
		}

		public async Task<GetDataReport> GetRevenue(DateTime dateTime)
		{
			var getAllDaysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
		   .Select(day => new DateTime(dateTime.Year, dateTime.Month, day)).ToList();

			var firstDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
			var lastDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

			var getData = from dp in _context.DieuPhoi
						  join vd in _context.VanDon
						  on dp.MaVanDon equals vd.MaVanDon
						  where dp.TrangThai == 20
						  && dp.CreatedTime.Date >= firstDateOfMonth.Date
						  && dp.CreatedTime.Date <= lastDateOfMonth.Date
						  select new { dp, vd };

			var listData = await getData.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.dp.CreatedTime.Date)).ToListAsync();

			var listSubfee = new List<arrDouble>();
			foreach (var date in getAllDaysInMonth)
			{
				foreach (var item in listData)
				{
					if (date.Date == item.dp.CreatedTime.Date)
					{
						double priceSf = 0;
						_context.SubFeePrice.Where(c => _context.SubFeeByContract.Where(y => y.MaDieuPhoi == item.dp.Id).Select(y => y.PriceId).Contains(c.PriceId)).ToList().ForEach(async c =>
						   {
							   priceSf += c.Price * await _currencyExchange.GetPriceTradeNow(c.PriceType);
						   });

						listSubfee.Add(new arrDouble
						{
							date = item.dp.CreatedTime.Date,
							value =
							await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).SumAsync(y => y.Price) +
							priceSf +
							((double)item.dp.DonGiaNcc * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeNcc))
						});
					}
				}
			}
			listSubfee = listSubfee.GroupBy(x => x.date).Select(x => new arrDouble()
			{
				date = x.Key.Date,
				value = x.Sum(c => c.value)
			}).ToList();
			listSubfee = listSubfee.Concat(getAllDaysInMonth.Where(x => !listSubfee.Select(y => y.date.Date).Contains(x.Date)).Select(x => new arrDouble()
			{
				date = x.Date,
				value = 0,
			})).ToList();

			var listRevenue = new List<arrDouble>();
			foreach (var date in getAllDaysInMonth)
			{
				foreach (var item in listData)
				{
					if (date.Date == item.dp.CreatedTime.Date)
					{
						double priceSf = 0;
						_context.SubFeePrice.Where(c => _context.SubFeeByContract.Where(y => y.MaDieuPhoi == item.dp.Id).Select(y => y.PriceId).Contains(c.PriceId)).ToList().ForEach(async c =>
					   {
						   priceSf += c.Price * await _currencyExchange.GetPriceTradeNow(c.PriceType);
					   });

						listRevenue.Add(new arrDouble
						{
							date = item.dp.CreatedTime.Date,
							value =
							await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).SumAsync(y => y.Price) +
							 priceSf +
							((double)item.dp.DonGiaKh * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeKh)),
						});
					}
				}
			};
			listRevenue = listRevenue.GroupBy(x => x.date).Select(x => new arrDouble()
			{
				date = x.Key.Date,
				value = x.Sum(c => c.value)
			}).ToList();
			listRevenue = listRevenue.Concat(getAllDaysInMonth.Where(x => !listRevenue.Select(y => y.date.Date).Contains(x.Date)).Select(x => new arrDouble()
			{
				date = x.Date,
				value = 0,
			})).ToList();

			var listProfit = new List<arrDouble>();
			foreach (var date in getAllDaysInMonth)
			{
				foreach (var item in listData)
				{
					if (date.Date == item.dp.CreatedTime.Date)
					{
						listProfit.Add(new arrDouble
						{
							date = item.dp.CreatedTime.Date,
							value = ((double)item.dp.DonGiaKh * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeKh)) - ((double)item.dp.DonGiaNcc * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeNcc))
						});
					}
				}
			};
			listProfit = listProfit.GroupBy(x => x.date).Select(x => new arrDouble()
			{
				date = x.Key.Date,
				value = x.Sum(c => c.value)
			}).ToList();
			listProfit = listProfit.Concat(getAllDaysInMonth.Where(x => !listProfit.Select(y => y.date.Date).Contains(x.Date)).Select(x => new arrDouble()
			{
				date = x.Date,
				value = 0,
			})).ToList();

			var arrSubfee = new DataReport()
			{
				Name = "Chi Phí",
				Color = "rgb(53, 162, 235)",
				DataDouble = listSubfee.ToList(),
			};

			var arrRevenue = new DataReport()
			{
				Name = "Doanh Thu",
				Color = "rgb(255, 99, 132)",
				DataDouble = listRevenue.ToList(),
			};

			var arrProfit = new DataReport()
			{
				Name = "Lợi Nhuận",
				Color = "rgba(233,157,79,255)",
				DataDouble = listProfit.ToList(),
			};

			var listTotal = new List<TotalReport>();
			listTotal.Add(new TotalReport { Title = "Tổng Chi Phí", TotalDouble = arrSubfee.DataDouble.Sum(x => x.value) });
			listTotal.Add(new TotalReport { Title = "Tổng Doanh Thu", TotalDouble = arrRevenue.DataDouble.Sum(x => x.value) });
			listTotal.Add(new TotalReport { Title = "Tổng Lợi Nhuận", TotalDouble = arrProfit.DataDouble.Sum(x => x.value) });

			return new GetDataReport()
			{
				Title = "Doanh Thu, Chi Phí Lợi Nhuận",
				TotalReports = listTotal,
				Labels = getAllDaysInMonth.Select(x => x.Date).ToList(),
				Data = new List<DataReport> { arrSubfee, arrRevenue, arrProfit },
			};
		}

		public async Task<TransportReport> GetCustomerReport(DateTime dateTime)
		{
			var getAllDaysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
			.Select(day => new DateTime(dateTime.Year, dateTime.Month, day)).ToList();

			var firstDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
			var lastDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

			var data = from vd in _context.VanDon
					   join dp in _context.DieuPhoi
					   on vd.MaVanDon equals dp.MaVanDon
					   where dp.TrangThai == 20
					   && dp.CreatedTime.Date >= firstDateOfMonth && dp.CreatedTime.Date <= lastDateOfMonth
					   select new { vd, dp };

			var listData = await data.ToListAsync();
			var listDataCustomer = new List<DataReportOfCustomer>();
			var listDataSuplier = new List<DataReportOfCustomer>();

			foreach (var item in listData)
			{
				double priceSf = 0;
				_context.SubFeePrice.Where(y => _context.SubFeeByContract.Where(z => z.MaDieuPhoi == item.dp.Id).Select(z => z.PriceId).Contains(y.PriceId) && y.CusType == "KH").ToList().ForEach(async x =>
				{
					priceSf = x.Price * await _currencyExchange.GetPriceTradeNow(x.PriceType);
				});

				listDataCustomer.Add(new DataReportOfCustomer()
				{
					customer = item.vd.MaKh,
					totalSf = ((double)item.dp.DonGiaNcc * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeNcc)) +
					await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).SumAsync(y => y.Price) +
					priceSf,
					totalMoney = await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).SumAsync(y => y.Price) +
					((double)item.dp.DonGiaKh * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeKh)) +
					priceSf,
					profit = ((double)item.dp.DonGiaKh.Value * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeKh)) - ((double)item.dp.DonGiaNcc.Value * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeNcc))
				});
			}

			foreach (var item in listData)
			{
				double priceSf = 0;
				_context.SubFeePrice.Where(y => _context.SubFeeByContract.Where(z => z.MaDieuPhoi == item.dp.Id).Select(z => z.PriceId).Contains(y.PriceId) && y.CusType == "NCC").ToList().ForEach(async x =>
				{
					priceSf = x.Price * await _currencyExchange.GetPriceTradeNow(x.PriceType);
				});

				listDataSuplier.Add(new DataReportOfCustomer()
				{
					customer = item.dp.DonViVanTai,
					totalSf = ((double)item.dp.DonGiaNcc * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeNcc)) +
					await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).SumAsync(y => y.Price) +
					priceSf,
					totalMoney = await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).SumAsync(y => y.Price) +
					((double)item.dp.DonGiaKh * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeKh)) +
					priceSf,
					profit = ((double)item.dp.DonGiaKh.Value * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeKh)) - ((double)item.dp.DonGiaNcc.Value * await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeNcc))
				});
			}

			var DataCustomer = listData.GroupBy(x => x.vd.MaKh).Select(x => new CustomerReport
			{
				CustomerName = _context.KhachHang.Where(v => v.MaKh == x.First().vd.MaKh).Select(v => v.TenKh).FirstOrDefault(),
				totalBooking = _context.VanDon.Where(c => x.Select(z => z.vd.MaVanDon).Contains(c.MaVanDon)).Count(),
				Total = x.Count(),
				CONT20 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT20"),
				CONT40 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT40"),
				CONT40RF = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT40RF"),
				CONT45 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT45"),
				TRUCK1 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1"),
				TRUCK15 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1.5"),
				TRUCK17 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1.7"),
				TRUCK10 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK10"),
				TRUCK150 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK15"),
				TRUCK2 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK2"),
				TRUCK25 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK2.5"),
				TRUCK3 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK3"),
				TRUCK35 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK3.5"),
				TRUCK5 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK5"),
				TRUCK7 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK7"),
				TRUCK8 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK8"),
				TRUCK9 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK9"),
				totalSf = listDataCustomer.Where(y => y.customer == x.Select(z => z.vd.MaKh).FirstOrDefault()).Sum(y => y.totalSf),
				totalMoney = listDataCustomer.Where(y => y.customer == x.Select(z => z.vd.MaKh).FirstOrDefault()).Sum(y => y.totalMoney),
				profit = listDataCustomer.Where(y => y.customer == x.Select(z => z.vd.MaKh).FirstOrDefault()).Sum(y => y.profit),
			}).OrderBy(x => x.CustomerName).ToList();

			var DataSupplier = listData.GroupBy(x => x.dp.DonViVanTai).Select(x => new CustomerReport
			{
				CustomerName = _context.KhachHang.Where(v => v.MaKh == x.First().dp.DonViVanTai).Select(v => v.TenKh).FirstOrDefault(),
				Total = x.Count(),
				CONT20 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT20"),
				CONT40 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT40"),
				CONT40RF = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT40RF"),
				CONT45 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT45"),
				TRUCK1 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1"),
				TRUCK15 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1.5"),
				TRUCK17 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1.7"),
				TRUCK10 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK10"),
				TRUCK150 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK15"),
				TRUCK2 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK2"),
				TRUCK25 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK2.5"),
				TRUCK3 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK3"),
				TRUCK35 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK3.5"),
				TRUCK5 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK5"),
				TRUCK7 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK7"),
				TRUCK8 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK8"),
				TRUCK9 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK9"),
				totalSf = listDataSuplier.Where(y => y.customer == x.Select(z => z.dp.DonViVanTai).FirstOrDefault()).Sum(y => y.totalSf),
				totalMoney = listDataSuplier.Where(y => y.customer == x.Select(z => z.dp.DonViVanTai).FirstOrDefault()).Sum(y => y.totalMoney),
				profit = listDataSuplier.Where(y => y.customer == x.Select(z => z.dp.DonViVanTai).FirstOrDefault()).Sum(y => y.profit),
			}).OrderBy(x => x.CustomerName).ToList();

			var listPartner = await _context.KhachHang.ToListAsync();
			return new TransportReport()
			{
				customerReports = DataCustomer,
				supllierReports = DataSupplier,
			};

			#region fix lấy thống kê theo kỳ

			//var data = from vd in _context.VanDon
			//		   join dp in _context.DieuPhoi
			//		   on vd.MaVanDon equals dp.MaVanDon
			//		   where dp.TrangThai == 20
			//		   select new { vd, dp };

			//var listDataCustomer = new List<DataReportOfCustomer>();
			//var listDataSuplier = new List<DataReportOfCustomer>();

			//var getListContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDongCha == null).ToListAsync();

			//foreach (var ct in getListContract.Where(x => x.MaKh.Substring(0, 3) == "CUS"))
			//{
			//	var getList = await data.Where(x => x.vd.MaKh == ct.MaKh
			//	&& x.dp.CreatedTime >= new DateTime(dateTime.Year, dateTime.Month, ct.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
			//	&& x.dp.CreatedTime <= new DateTime(dateTime.Year, dateTime.Month, ct.NgayThanhToan.Value)).ToListAsync();

			//	foreach (var item in getList)
			//	{
			//		double priceSf = 0;
			//		_context.SubFeePrice.Where(y => _context.SubFeeByContract.Where(z => z.MaDieuPhoi == item.dp.Id).Select(z => z.PriceId).Contains(y.PriceId) && y.CusType == "KH").ToList().ForEach(async x =>
			//		{
			//			priceSf = x.Price * await _priceTable.GetPriceTradeNow(x.PriceType);
			//		});
			//		double sfI = await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).SumAsync(y => y.Price);
			//		double priceCus = await _priceTable.GetPriceTradeNow(item.dp.LoaiTienTeKh);
			//		double priceSup = await _priceTable.GetPriceTradeNow(item.dp.LoaiTienTeNcc);

			//		listDataCustomer.Add(new DataReportOfCustomer()
			//		{
			//			customer = item.vd.MaKh,
			//			totalSf = ((double)item.dp.DonGiaNcc * priceSup) + sfI + priceSf,
			//			totalMoney = sfI + ((double)item.dp.DonGiaKh * priceCus) + priceSf,
			//			profit = ((double)item.dp.DonGiaKh.Value * priceCus) - ((double)item.dp.DonGiaNcc.Value * priceSup)
			//		});
			//	}
			//}

			//foreach (var ct in getListContract.Where(x => x.MaKh.Substring(0, 3) == "SUP"))
			//{
			//	var getList = await data.Where(x => x.dp.DonViVanTai == ct.MaKh
			//	&& x.dp.CreatedTime >= new DateTime(dateTime.Year, dateTime.Month, ct.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
			//	&& x.dp.CreatedTime <= new DateTime(dateTime.Year, dateTime.Month, ct.NgayThanhToan.Value)).ToListAsync();

			//	foreach (var item in getList)
			//	{
			//		double priceSf = 0;
			//		_context.SubFeePrice.Where(y => _context.SubFeeByContract.Where(z => z.MaDieuPhoi == item.dp.Id).Select(z => z.PriceId).Contains(y.PriceId) && y.CusType == "NCC").ToList().ForEach(async x =>
			//		{
			//			priceSf = x.Price * await _priceTable.GetPriceTradeNow(x.PriceType);
			//		});
			//		double sfI = await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).SumAsync(y => y.Price);
			//		double priceCus = await _priceTable.GetPriceTradeNow(item.dp.LoaiTienTeKh);
			//		double priceSup = await _priceTable.GetPriceTradeNow(item.dp.LoaiTienTeNcc);

			//		listDataSuplier.Add(new DataReportOfCustomer()
			//		{
			//			customer = item.dp.DonViVanTai,
			//			totalSf = ((double)item.dp.DonGiaNcc * priceSup) + sfI + priceSf,
			//			totalMoney = sfI + ((double)item.dp.DonGiaKh * priceCus) + priceSf,
			//			profit = ((double)item.dp.DonGiaKh.Value * priceCus) - ((double)item.dp.DonGiaNcc.Value * priceSup)
			//		});
			//	}
			//}

			//var DataCustomer = new List<CustomerReport>();
			//var DataSupplier = new List<CustomerReport>();

			//foreach (var ct in getListContract.Where(x => x.MaKh.Substring(0, 3) == "CUS"))
			//{
			//	var getDataCus = await data.Where(x => x.vd.MaKh == ct.MaKh
			//	   && x.dp.CreatedTime >= new DateTime(dateTime.Year, dateTime.Month, ct.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
			//	   && x.dp.CreatedTime <= new DateTime(dateTime.Year, dateTime.Month, ct.NgayThanhToan.Value)).ToListAsync();

			//	if (getDataCus.Count > 0)
			//	{
			//		DataCustomer.Add(getDataCus.GroupBy(x => x.vd.MaKh).Select(x => new CustomerReport
			//		{
			//			CustomerName = _context.KhachHang.Where(v => v.MaKh == ct.MaKh).Select(v => v.TenKh).FirstOrDefault(),
			//			totalBooking = _context.VanDon.Where(c => x.Select(z => z.vd.MaVanDon).Contains(c.MaVanDon)).Count(),
			//			Total = x.Count(),
			//			CONT20 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT20"),
			//			CONT40 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT40"),
			//			CONT40RF = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT40RF"),
			//			CONT45 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT45"),
			//			TRUCK1 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1"),
			//			TRUCK15 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1.5"),
			//			TRUCK17 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1.7"),
			//			TRUCK10 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK10"),
			//			TRUCK150 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK15"),
			//			TRUCK2 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK2"),
			//			TRUCK25 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK2.5"),
			//			TRUCK3 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK3"),
			//			TRUCK35 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK3.5"),
			//			TRUCK5 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK5"),
			//			TRUCK7 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK7"),
			//			TRUCK8 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK8"),
			//			TRUCK9 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK9"),
			//			totalSf = listDataCustomer.Where(y => y.customer == x.Select(z => z.vd.MaKh).FirstOrDefault()).Sum(y => y.totalSf),
			//			totalMoney = listDataCustomer.Where(y => y.customer == x.Select(z => z.vd.MaKh).FirstOrDefault()).Sum(y => y.totalMoney),
			//			profit = listDataCustomer.Where(y => y.customer == x.Select(z => z.vd.MaKh).FirstOrDefault()).Sum(y => y.profit),
			//		}).FirstOrDefault());
			//	}
			//}

			//foreach (var ct in getListContract.Where(x => x.MaKh.Substring(0, 3) == "SUP"))
			//{
			//	var getDataSup = await data.Where(x => x.dp.DonViVanTai == ct.MaKh
			//	   && x.dp.CreatedTime >= new DateTime(dateTime.Year, dateTime.Month, ct.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
			//	   && x.dp.CreatedTime <= new DateTime(dateTime.Year, dateTime.Month, ct.NgayThanhToan.Value)).ToListAsync();

			//	if (getDataSup.Count > 0)
			//	{
			//		DataSupplier.Add(getDataSup.GroupBy(x => x.dp.DonViVanTai).Select(x => new CustomerReport
			//		{
			//			CustomerName = _context.KhachHang.Where(v => v.MaKh == ct.MaKh).Select(v => v.TenKh).FirstOrDefault(),
			//			Total = x.Count(),
			//			CONT20 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT20"),
			//			CONT40 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT40"),
			//			CONT40RF = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT40RF"),
			//			CONT45 = x.Count(c => c.dp.MaLoaiPhuongTien == "CONT45"),
			//			TRUCK1 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1"),
			//			TRUCK15 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1.5"),
			//			TRUCK17 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK1.7"),
			//			TRUCK10 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK10"),
			//			TRUCK150 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK15"),
			//			TRUCK2 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK2"),
			//			TRUCK25 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK2.5"),
			//			TRUCK3 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK3"),
			//			TRUCK35 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK3.5"),
			//			TRUCK5 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK5"),
			//			TRUCK7 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK7"),
			//			TRUCK8 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK8"),
			//			TRUCK9 = x.Count(c => c.dp.MaLoaiPhuongTien == "TRUCK9"),
			//			totalSf = listDataSuplier.Where(y => y.customer == x.Select(z => z.dp.DonViVanTai).FirstOrDefault()).Sum(y => y.totalSf),
			//			totalMoney = listDataSuplier.Where(y => y.customer == x.Select(z => z.dp.DonViVanTai).FirstOrDefault()).Sum(y => y.totalMoney),
			//			profit = listDataSuplier.Where(y => y.customer == x.Select(z => z.dp.DonViVanTai).FirstOrDefault()).Sum(y => y.profit),
			//		}).FirstOrDefault());
			//	}
			//}

			//return new TransportReport()
			//{
			//	customerReports = DataCustomer.OrderBy(x => x.CustomerName).ToList(),
			//	supllierReports = DataSupplier.OrderBy(x => x.CustomerName).ToList(),
			//};

			#endregion fix lấy thống kê theo kỳ
		}
	}
}