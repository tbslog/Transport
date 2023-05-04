using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.Bill
{
    public class BillService : IBill
    {
        private readonly TMSContext _context;
        private readonly ICommon _common;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public BillService(TMSContext context, ICommon common, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _common = common;
            _httpContextAccessor = httpContextAccessor;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }

        public async Task<GetBill> GetBillByCustomerId(string customerId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                //var checkexists = await GetListKyThanhToan(customerId);
                //if (checkexists == null)
                //{
                //    return null;
                //}

                var getlistHandling = from dp in _context.DieuPhoi
                                      join vd in _context.VanDon
                                      on dp.MaVanDon equals vd.MaVanDon
                                      where (vd.MaKh == customerId || dp.DonViVanTai == customerId)
                                      && dp.TrangThai == 20
                                      && dp.CreatedTime.Date >= fromDate.Date
                                      && dp.CreatedTime.Date <= toDate.Date
                                      select dp;

                var getDataTransport = from kh in _context.KhachHang
                                       join vd in _context.VanDon
                                       on kh.MaKh equals vd.MaKh
                                       where getlistHandling.Select(x => x.MaVanDon).Contains(vd.MaVanDon)
                                       orderby vd.CreatedTime
                                       select new { kh, vd };

                var getListSFOfContract = from sfp in _context.SubFeePrice
                                          join sfc in _context.SubFeeByContract
                                          on sfp.PriceId equals sfc.PriceId
                                          join sf in _context.SubFee
                                          on sfp.SfId equals sf.SubFeeId
                                          select new { sfp, sfc, sf };

                var getListTransport = await getDataTransport.Where(x => getlistHandling.Select(s => s.MaVanDon).Contains(x.vd.MaVanDon)).OrderBy(x => x.vd.MaVanDonKh).Select(z => new ListVanDon()
                {
                    MaVanDonKH = z.vd.MaVanDonKh,
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    MaVanDon = z.vd.MaVanDon,
                    MaKh = z.kh.MaKh,
                    TenKh = z.kh.TenKh,
                    AccountName = z.vd.MaAccount == null ? _context.KhachHang.Where(y => y.MaKh == z.vd.MaKh).Select(y => y.TenKh).FirstOrDefault() : _context.AccountOfCustomer.Where(y => y.MaAccount == z.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
                    LoaiVanDon = z.vd.LoaiVanDon == "xuat" ? "Xuất" : "Nhập",
                    TongTheTich = z.vd.TongTheTich,
                    TongKhoiLuong = z.vd.TongKhoiLuong,
                    TongSoKien = z.vd.TongSoKien,
                    listHandling = getlistHandling.Where(y => y.MaVanDon == z.vd.MaVanDon).OrderBy(x => x.Id).Select(x => new Model.Model.BillModel.ListHandling()
                    {
                        MaSoXe = x.MaSoXe,
                        DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                        DiemTraRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                        MaRomooc = x.MaRomooc,
                        TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
                        LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
                        LoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
                        DonViTinh = _context.DonViTinh.Where(y => y.MaDvt == x.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
                        DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.DonViVanTai).Select(x => x.TenKh).FirstOrDefault(),
                        DonGia = x.DonGiaKh,
                        KhoiLuong = x.KhoiLuong,
                        TheTich = x.TheTich,
                        SoKien = x.SoKien,
                        listSubFeeByContract = getListSFOfContract.Where(y => y.sfc.MaDieuPhoi == x.Id).Select(x => new ListSubFeeByContract()
                        {
                            ContractId = x.sfp.ContractId,
                            ContractName = _context.HopDongVaPhuLuc.Where(c => c.MaHopDong == x.sfp.ContractId).Select(c => c.TenHienThi).FirstOrDefault(),
                            sfName = x.sf.SfName,
                            goodsType = x.sfp.GoodsType,
                            unitPrice = x.sfp.Price
                        }).ToList(),
                        listSubFeeIncurreds = _context.SfeeByTcommand.Where(y => y.IdTcommand == x.Id && y.ApproveStatus == 14).OrderBy(x => x.Id).Select(x => new ListSubFeeIncurred()
                        {
                            Note = x.Note,
                            Price = x.Price,
                            sfName = _context.SubFee.Where(y => y.SubFeeId == x.SfId).Select(x => x.SfName).FirstOrDefault()
                        }).ToList(),
                    }).ToList(),
                }).ToListAsync();

                return new GetBill()
                {
                    BillReuslt = getListTransport
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GetBill> GetBillByTransportId(string transportId, long? handlingId)
        {
            try
            {
                var getlistHandling = from dp in _context.DieuPhoi
                                      join vd in _context.VanDon
                                      on dp.MaVanDon equals vd.MaVanDon
                                      where dp.TrangThai == 20
                                      select dp;

                var getDataTransport = from kh in _context.KhachHang
                                       join vd in _context.VanDon
                                       on kh.MaKh equals vd.MaKh
                                       orderby vd.CreatedTime
                                       select new { kh, vd };

                var getListSFOfContract = from sfp in _context.SubFeePrice
                                          join sfc in _context.SubFeeByContract
                                          on sfp.PriceId equals sfc.PriceId
                                          join sf in _context.SubFee
                                          on sfp.SfId equals sf.SubFeeId
                                          select new { sfp, sfc, sf };

                if (handlingId != null)
                {
                    getlistHandling = getlistHandling.Where(x => x.Id == handlingId);
                }

                if (!string.IsNullOrEmpty(transportId))
                {
                    getDataTransport = getDataTransport.Where(x => x.vd.MaVanDon == transportId);
                }
                else
                {
                    getDataTransport = getDataTransport.Where(x => x.vd.MaVanDon == getlistHandling.Select(y => y.MaVanDon).FirstOrDefault());
                }

                var getListTransport = await getDataTransport.Select(z => new ListVanDon()
                {
                    MaVanDonKH = z.vd.MaVanDonKh,
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    MaVanDon = z.vd.MaVanDon,
                    MaKh = z.kh.MaKh,
                    TenKh = z.kh.TenKh,
                    AccountName = z.vd.MaAccount == null ? _context.KhachHang.Where(y => y.MaKh == z.vd.MaKh).Select(y => y.TenKh).FirstOrDefault() : _context.AccountOfCustomer.Where(y => y.MaAccount == z.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
                    LoaiVanDon = z.vd.LoaiVanDon == "xuat" ? "Xuất" : "Nhập",
                    TongTheTich = z.vd.TongTheTich,
                    TongKhoiLuong = z.vd.TongKhoiLuong,
                    TongSoKien = z.vd.TongSoKien,
                    listHandling = getlistHandling.Where(y => y.MaVanDon == z.vd.MaVanDon).OrderBy(x => x.Id).Select(x => new Model.Model.BillModel.ListHandling()
                    {
                        MaSoXe = x.MaSoXe,
                        DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                        DiemTraRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                        MaRomooc = x.MaRomooc,
                        TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
                        LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
                        LoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
                        DonViTinh = _context.DonViTinh.Where(y => y.MaDvt == x.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
                        DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.DonViVanTai).Select(x => x.TenKh).FirstOrDefault(),
                        DonGia = x.DonGiaKh,
                        KhoiLuong = x.KhoiLuong,
                        TheTich = x.TheTich,
                        SoKien = x.SoKien,
                        listSubFeeByContract = getListSFOfContract.Where(y => y.sfc.MaDieuPhoi == x.Id).Select(x => new ListSubFeeByContract()
                        {
                            ContractId = x.sfp.ContractId,
                            ContractName = _context.HopDongVaPhuLuc.Where(c => c.MaHopDong == x.sfp.ContractId).Select(c => c.TenHienThi).FirstOrDefault(),
                            sfName = x.sf.SfName,
                            goodsType = x.sfp.GoodsType,
                            unitPrice = x.sfp.Price
                        }).ToList(),
                        listSubFeeIncurreds = _context.SfeeByTcommand.Where(y => y.IdTcommand == x.Id && y.ApproveStatus == 14).OrderBy(x => x.Id).Select(x => new ListSubFeeIncurred()
                        {
                            Note = x.Note,
                            Price = x.Price,
                            sfName = _context.SubFee.Where(y => y.SubFeeId == x.SfId).Select(x => x.SfName).FirstOrDefault()
                        }).ToList(),
                    }).ToList(),
                }).ToListAsync();

                return new GetBill()
                {
                    BillReuslt = getListTransport
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResponseCustom<ListVanDon>> GetListTransportByCustomerId(string customerId, int ky, PaginationFilter filter)
        {
            var checkexists = await GetListKyThanhToan(customerId);

            if (checkexists == null)
            {
                return null;
            }

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var kyThanhToan = checkexists.Where(x => x.Ky == ky).FirstOrDefault();

            var getlistHandling = from dp in _context.DieuPhoi
                                  join vd in _context.VanDon
                                  on dp.MaVanDon equals vd.MaVanDon
                                  where
                                  (vd.MaKh == customerId || dp.DonViVanTai == customerId)
                                  && dp.TrangThai == 20
                                  && dp.CreatedTime.Date >= kyThanhToan.StartDate.Date
                                  && dp.CreatedTime.Date <= kyThanhToan.EndDate.Date
                                  select dp;

            var listData = from kh in _context.KhachHang
                           join vd in _context.VanDon
                           on kh.MaKh equals vd.MaKh
                           where getlistHandling.Select(x => x.MaVanDon).Contains(vd.MaVanDon)
                           orderby vd.CreatedTime
                           select new { kh, vd };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword));
            }

            var totalCount = await listData.CountAsync();

            var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListVanDon()
            {
                MaVanDon = x.vd.MaVanDon,
                MaKh = x.kh.MaKh,
                TenKh = x.kh.TenKh,
                LoaiVanDon = x.vd.LoaiVanDon,
                DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                TongTheTich = x.vd.TongTheTich.Value,
                TongKhoiLuong = x.vd.TongKhoiLuong.Value
            }).OrderBy(x => x.MaKh).ToListAsync();

            return new PagedResponseCustom<ListVanDon>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<List<KyThanhToan>> GetListKyThanhToan(string customerId)
        {
            var getContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == customerId && x.MaHopDongCha == null && x.NgayThanhToan != null).FirstOrDefaultAsync();

            if (getContract == null)
            {
                return null;
            }

            var ngayThanhToan = getContract.NgayThanhToan.Value;
            var dateNow = DateTime.Now.Date;
            var timeStart = getContract.ThoiGianBatDau.Date;
            var allDates = new List<DateTime>();

            for (DateTime date = timeStart; date <= dateNow; date = date.AddDays(1))
            {
                allDates.Add(date);
            }

            var listKy = new List<KyThanhToan>();
            int count = 0;
            foreach (var date in allDates)
            {
                if (date.Date.Day == ngayThanhToan)
                {
                    count += 1;

                    listKy.Add(new KyThanhToan()
                    {
                        Ky = count,
                        StartDate = date.Date.AddDays(1),
                        EndDate = new DateTime(date.Date.Year, date.Date.AddMonths(1).Month, ngayThanhToan)
                    });
                }
            }

            return listKy;
        }

        public async Task<PagedResponseCustom<ListBillTransportWeb>> GetListBillWeb(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var getlistHandling = from dp in _context.DieuPhoi
                                  join vd in _context.VanDon
                                  on dp.MaVanDon equals vd.MaVanDon
                                  where dp.TrangThai == 20
                                  select new { dp, vd };

            var getListSFOfContract = from sfp in _context.SubFeePrice
                                      join sfc in _context.SubFeeByContract
                                      on sfp.PriceId equals sfc.PriceId
                                      select new { sfp, sfc };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                getlistHandling = getlistHandling.Where(x => x.vd.MaVanDonKh.Contains(filter.Keyword) || x.vd.MaKh.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.customerId))
            {
                getlistHandling = getlistHandling.Where(x => x.vd.MaKh == filter.customerId);
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                getlistHandling = getlistHandling.Where(x => x.dp.CreatedTime.Date >= filter.fromDate.Value && x.dp.CreatedTime.Date <= filter.toDate.Value);
            }

            var totalCount = await getlistHandling.CountAsync();

            var pagedData = getlistHandling.OrderBy(x => x.vd.MaVanDon).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize);

            var getlistTransport = await _context.VanDon.Where(x => pagedData.Select(y => y.vd.MaVanDon).Contains(x.MaVanDon)).OrderByDescending(x => x.MaVanDon).Select(x => new ListBillTransportWeb()
            {
                MaVanDon = x.MaVanDon,
                BookingNo = x.MaVanDonKh,
                TongTien = pagedData.Where(y => y.dp.MaVanDon == x.MaVanDon).Sum(y => y.dp.DonGiaKh.Value),
                TongPhuPhi = getListSFOfContract.Where(y => pagedData.Where(c => c.vd.MaVanDon == x.MaVanDon).Select(c => c.dp.Id).Contains(y.sfc.MaDieuPhoi)).Sum(y => y.sfp.Price)
                + _context.SfeeByTcommand.Where(y => pagedData.Where(c => c.vd.MaVanDon == x.MaVanDon).Select(c => c.dp.Id).Contains(y.IdTcommand) && y.ApproveStatus == 14).Sum(y => y.Price),
                HangTau = x.HangTau == null ? "" : _context.ShippingInfomation.Where(y => y.ShippingCode == x.HangTau).Select(y => y.ShippingLineName).FirstOrDefault(),
                TenKH = _context.KhachHang.Where(y => y.MaKh == x.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                Account = x.MaAccount == null ? "" : _context.AccountOfCustomer.Where(y => y.MaAccount == x.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
                LoaiVanDon = x.LoaiVanDon.Trim() == "xuat" ? "Xuất" : "Nhập",
                MaPTVC = x.MaPtvc,
                listBillHandlingWebs = getlistHandling.Where(y => y.dp.MaVanDon == x.MaVanDon).Select(z => new ListBillHandlingWeb()
                {
                    handlingId = z.dp.Id,
                    DonViVanTai = _context.KhachHang.Where(y => y.MaKh == z.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
                    DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == z.dp.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
                    LoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == z.dp.MaLoaiPhuongTien).Select(y => y.TenLoaiPhuongTien).FirstOrDefault(),
                    MaSoXe = z.dp.MaSoXe,
                    TaiXe = z.dp.MaTaiXe,
                    DonGiaKH = z.dp.DonGiaKh.Value,
                    PhuPhiHD = _context.SubFeeByContract.Where(y => y.MaDieuPhoi == z.dp.Id).Select(y => y.Price).Sum(y => y.Price),
                    PhuPhiPhatSinh = _context.SfeeByTcommand.Where(y => y.IdTcommand == z.dp.Id && y.ApproveStatus == 14).Sum(y => y.Price),
                    ContNo = z.dp.ContNo,
                    SealNP = z.dp.SealNp,
                    SealHQ = z.dp.SealHq,
                    DiemLayRong = z.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == z.dp.DiemLayRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    DiemTraRong = z.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == z.dp.DiemTraRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
                }).ToList(),
            }).ToListAsync();

            return new PagedResponseCustom<ListBillTransportWeb>()
            {
                dataResponse = getlistTransport,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<PagedResponseCustom<ListBillHandling>> GetListBillHandling(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var getlistHandling = from dp in _context.DieuPhoi
                                  join vd in _context.VanDon
                                  on dp.MaVanDon equals vd.MaVanDon
                                  where dp.TrangThai == 20
                                  select new { dp, vd };

            var getSFbyContract = from sfc in _context.SubFeeByContract
                                  join sfp in _context.SubFeePrice
                                  on sfc.PriceId equals sfp.PriceId
                                  select new { sfc, sfp };


            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                getlistHandling = getlistHandling.Where(x => x.vd.MaVanDonKh.Contains(filter.Keyword) || x.dp.MaChuyen.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                getlistHandling = getlistHandling.Where(x => x.dp.CreatedTime.Date >= filter.fromDate.Value && x.dp.CreatedTime.Date <= filter.toDate.Value);
            }

            if (!string.IsNullOrEmpty(filter.customerId))
            {
                getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == filter.customerId || x.vd.MaKh == filter.customerId);
            }

            var totalCount = await getlistHandling.CountAsync();

            var pagedData = await getlistHandling.OrderByDescending(x => x.dp.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListBillHandling()
            {
                HangTau = x.vd.HangTau,
                handlingId = x.dp.Id,
                ContNo = x.dp.ContNo,
                MaChuyen = x.dp.MaChuyen,
                AccountName = x.vd.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == x.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
                CutOffDate = x.vd.ThoiGianHaCang,
                DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                DiemLayRong = x.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                DiemTraRong = x.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                MaPTVC = x.vd.MaPtvc,
                MaVanDonKH = x.vd.MaVanDonKh,
                MaVanDon = x.dp.MaVanDon,
                LoaiVanDon = x.vd.LoaiVanDon,
                LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.dp.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
                LoaiPhuongTien = x.dp.MaLoaiPhuongTien,
                MaNCC = x.dp.DonViVanTai,
                MaKH = x.vd.MaKh,
                TenKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                TenNCC = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
                DonGiaKH = x.dp.DonGiaKh,
                DonGiaNCC = x.dp.DonGiaNcc,
                LoiNhuan = x.dp.DonGiaKh.Value - x.dp.DonGiaNcc.Value,
                createdTime = x.dp.CreatedTime,
                ChiPhiHopDong = (decimal)getSFbyContract.Where(y => y.sfc.MaDieuPhoi == x.dp.Id).Sum(y => y.sfp.Price),
                ChiPhiPhatSinh = (decimal)_context.SfeeByTcommand.Where(y => y.IdTcommand == x.dp.Id && y.ApproveStatus == 14).Sum(y => y.Price),
            }).Select(x => new ListBillHandling()
            {
                createdTime = x.createdTime,
                HangTau = x.HangTau,
                MaVanDon = "",
                handlingId = x.handlingId,
                ContNo = x.ContNo,
                AccountName = x.AccountName == null ? x.TenKH : x.AccountName,
                CutOffDate = x.CutOffDate,
                DiemDau = x.DiemDau,
                DiemCuoi = x.DiemCuoi,
                DiemLayRong = x.DiemLayRong,
                DiemTraRong = x.DiemTraRong,
                MaPTVC = x.MaPTVC,
                LoaiVanDon = x.LoaiVanDon,
                MaVanDonKH = x.MaVanDonKH,
                MaChuyen = x.MaChuyen,
                LoaiHangHoa = x.LoaiHangHoa,
                LoaiPhuongTien = x.LoaiPhuongTien,
                MaNCC = x.MaNCC,
                MaKH = x.MaKH,
                TenKH = x.TenKH,
                TenNCC = x.TenNCC,
                DonGiaKH = x.DonGiaKH,
                DonGiaNCC = x.DonGiaNCC,
                DoanhThu = x.DonGiaKH.Value + x.ChiPhiPhatSinh + x.ChiPhiHopDong,
                LoiNhuan = x.LoiNhuan,
                ChiPhiHopDong = x.ChiPhiHopDong,
                ChiPhiPhatSinh = x.ChiPhiPhatSinh,
            }).OrderByDescending(x => x.MaVanDon).ThenBy(x => x.MaChuyen).ToListAsync();

            return new PagedResponseCustom<ListBillHandling>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }
    }
}