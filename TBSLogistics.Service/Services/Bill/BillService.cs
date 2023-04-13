using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
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

        public async Task<GetBill> GetBillByCustomerId(string customerId, int ky)
        {
            try
            {
                var checkexists = await GetListKyThanhToan(customerId);

                if (checkexists == null)
                {
                    return null;
                }

                var kyThanhToan = checkexists.Where(x => x.Ky == ky).FirstOrDefault();

                var getlistHandling = from dp in _context.DieuPhoi
                                      join vd in _context.VanDon
                                      on dp.MaVanDon equals vd.MaVanDon
                                      where
                                      (vd.MaKh == customerId || dp.DonViVanTai == customerId)
                                      && dp.TrangThai == 20
                                      && dp.ThoiGianHoanThanh.Value.Date >= kyThanhToan.StartDate
                                      && dp.ThoiGianHoanThanh.Value.Date <= kyThanhToan.EndDate
                                      select dp;

                var getDataTransport = from kh in _context.KhachHang
                                       join vd in _context.VanDon
                                       on kh.MaKh equals vd.MaKh
                                       where getlistHandling.Select(x => x.MaVanDon).Contains(vd.MaVanDon)
                                       orderby vd.ThoiGianHoanThanh
                                       select new { kh, vd };

                var getListSubFeeByContract = from kh in _context.KhachHang
                                              join hd in _context.HopDongVaPhuLuc
                                              on kh.MaKh equals hd.MaKh
                                              join sfPice in _context.SubFeePrice
                                              on hd.MaHopDong equals sfPice.ContractId
                                              join sf in _context.SubFee
                                              on sfPice.SfId equals sf.SubFeeId
                                              where sfPice.Status == 14
                                              && kh.MaKh == customerId
                                              select new { kh, hd, sfPice, sf };

                var getListTransport = await getDataTransport.Where(x => getlistHandling.Select(s => s.MaVanDon).Contains(x.vd.MaVanDon)).OrderBy(x => x.vd.MaVanDon).Select(z => new ListVanDon()
                {
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    AccountName = z.vd.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == z.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
                    MaVanDon = z.vd.MaVanDon,
                    MaKh = z.vd.MaKh,
                    TenKh = z.kh.TenKh,
                    LoaiVanDon = z.vd.LoaiVanDon,
                    TongTheTich = z.vd.TongTheTich.Value,
                    TongKhoiLuong = z.vd.TongKhoiLuong.Value,
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
                        DonGia = _context.KhachHang.Where(x => x.MaKh == customerId).Select(x => x.MaLoaiKh).FirstOrDefault() == "NCC" ? x.DonGiaNcc : x.DonGiaKh,
                        KhoiLuong = x.KhoiLuong,
                        TheTich = x.TheTich,
                        listSubFeeByContract = getListSubFeeByContract.Where(y => (y.sfPice.GoodsType == x.MaLoaiHangHoa)
                        || (y.sfPice.GetEmptyPlace == _context.DiaDiem.Where(o => o.MaDiaDiem == (z.vd.LoaiVanDon == "xuat" ? x.DiemLayRong : x.DiemTraRong)).Select(o => o.MaDiaDiem).FirstOrDefault())
                        || (y.sfPice.FirstPlace == z.vd.DiemDau && y.sfPice.SecondPlace == z.vd.DiemCuoi)
                        || (y.sfPice.GoodsType == null && (y.sfPice.FirstPlace == null && y.sfPice.SecondPlace == null) && y.sfPice.GetEmptyPlace == null)
                        ).OrderBy(x => x.sfPice).Select(x => new ListSubFeeByContract()
                        {
                            ContractId = x.hd.MaHopDong,
                            ContractName = x.hd.TenHienThi,
                            sfName = x.sf.SfName,
                            goodsType = x.sfPice.GoodsType,
                            //TripName = _context.CungDuong.Where(y => y.MaCungDuong == x.sfPice.TripId).Select(x => x.TenCungDuong).FirstOrDefault(),
                            //AreaName = _context.KhuVuc.Where(y => y.Id == x.sfPice.AreaId).Select(x => x.TenKhuVuc).FirstOrDefault(),
                            unitPrice = x.sfPice.Price
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

        public async Task<GetBill> GetBillByTransportId(string customerId, string transportId)
        {
            try
            {
                var getlistHandling = from dp in _context.DieuPhoi
                                      join vd in _context.VanDon
                                      on dp.MaVanDon equals vd.MaVanDon
                                      where (vd.MaKh == customerId || dp.DonViVanTai == customerId)
                                      && dp.TrangThai == 20
                                      select dp;

                var getDataTransport = from kh in _context.KhachHang
                                       join vd in _context.VanDon
                                       on kh.MaKh equals vd.MaKh
                                       orderby vd.ThoiGianHoanThanh
                                       select new { kh, vd };

                var getSFbyContract = from sfc in _context.SubFeeByContract
                                      join sfp in _context.SubFeePrice
                                      on sfc.PriceId equals sfp.PriceId
                                      join sf in _context.SubFee
                                      on sfp.SfId equals sf.SubFeeId
                                      join ct in _context.HopDongVaPhuLuc
                                      on sfp.ContractId equals ct.MaHopDong into ctp
                                      from ctsf in ctp.DefaultIfEmpty()
                                      select new { sfc, sfp, sf, ctsf };

                var getListTransport = await getDataTransport.Where(x => x.vd.MaVanDon == transportId).Select(z => new ListVanDon()
                {
                    MaVanDonKH = z.vd.MaVanDonKh,
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    MaVanDon = z.vd.MaVanDon,
                    MaKh = z.kh.MaKh,
                    TenKh = z.kh.TenKh,
                    AccountName = z.vd.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == z.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
                    LoaiVanDon = z.vd.LoaiVanDon,
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
                        DonGia = _context.KhachHang.Where(x => x.MaKh == customerId).Select(x => x.MaLoaiKh).FirstOrDefault() == "NCC" ? x.DonGiaNcc : x.DonGiaKh,
                        KhoiLuong = x.KhoiLuong,
                        TheTich = x.TheTich,
                        SoKien = x.SoKien,
                        listSubFeeByContract = getSFbyContract.Where(y => y.sfc.MaDieuPhoi == x.Id).Select(x => new ListSubFeeByContract()
                        {
                            ContractId = x.ctsf.MaHopDong,
                            ContractName = x.ctsf.TenHienThi,
                            sfName = x.sf.SfName,
                            goodsType = x.sfp.GoodsType,
                            //TripName = _context.CungDuong.Where(y => y.MaCungDuong == x.sfp.TripId).Select(x => x.TenCungDuong).FirstOrDefault(),
                            //AreaName = _context.KhuVuc.Where(y => y.Id == x.sfp.AreaId).Select(x => x.TenKhuVuc).FirstOrDefault(),
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
                                  && dp.ThoiGianHoanThanh.Value.Date >= kyThanhToan.StartDate.Date
                                  && dp.ThoiGianHoanThanh.Value.Date <= kyThanhToan.EndDate.Date
                                  select dp;

            var listData = from kh in _context.KhachHang
                           join vd in _context.VanDon
                           on kh.MaKh equals vd.MaKh
                           where getlistHandling.Select(x => x.MaVanDon).Contains(vd.MaVanDon)
                           orderby vd.ThoiGianHoanThanh
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
                getlistHandling = getlistHandling.Where(x => x.dp.ThoiGianHoanThanh.Value >= filter.fromDate.Value && x.dp.ThoiGianHoanThanh.Value <= filter.toDate.Value);
            }

            if (!string.IsNullOrEmpty(filter.customerId))
            {
                getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == filter.customerId || x.vd.MaKh == filter.customerId);
            }

            var totalCount = await getlistHandling.CountAsync();

            var pagedData = await getlistHandling.OrderByDescending(x => x.dp.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListBillHandling()
            {
                MaChuyen = x.dp.Id,
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
                ChiPhiHopDong = (decimal)getSFbyContract.Where(y => y.sfc.MaDieuPhoi == x.dp.Id).Sum(y => y.sfp.Price),
                ChiPhiPhatSinh = (decimal)_context.SfeeByTcommand.Where(y => y.IdTcommand == x.dp.Id && y.ApproveStatus == 14).Sum(y => y.Price),
            }).Select(x => new ListBillHandling()
            {
                AccountName = x.AccountName == null ? x.TenKH : x.AccountName,
                CutOffDate = x.CutOffDate,
                DiemDau = x.DiemDau,
                DiemCuoi = x.DiemCuoi,
                DiemLayRong = x.DiemLayRong,
                DiemTraRong = x.DiemTraRong,
                MaPTVC = x.MaPTVC,
                LoaiVanDon = x.LoaiVanDon,
                MaVanDonKH = x.MaVanDonKH,
                MaVanDon = x.MaVanDon,
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