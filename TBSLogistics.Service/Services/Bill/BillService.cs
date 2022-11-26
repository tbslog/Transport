using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;
using TBSLogistics.Service.Services.RomoocManage;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TBSLogistics.Service.Services.Bill
{
    public class BillService : IBill
    {
        private readonly TMSContext _context;
        private readonly ICommon _common;

        public BillService(TMSContext context, ICommon common)
        {
            _context = context;
            _common = common;
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
                                       join cd in _context.CungDuong
                                       on vd.MaCungDuong equals cd.MaCungDuong
                                       where getlistHandling.Select(x => x.MaVanDon).Contains(vd.MaVanDon)
                                       orderby vd.ThoiGianHoanThanh
                                       select new { kh, vd, cd };

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
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.cd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.cd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    MaVanDon = z.vd.MaVanDon,
                    MaKh = z.vd.MaKh,
                    TenKh = z.kh.TenKh,
                    LoaiVanDon = z.vd.LoaiVanDon,
                    MaCungDuong = z.cd.MaCungDuong,
                    TenCungDuong = z.cd.TenCungDuong,
                    TongTheTich = z.vd.TongTheTich,
                    TongKhoiLuong = z.vd.TongKhoiLuong,
                    listHandling = getlistHandling.Where(y => y.MaVanDon == z.vd.MaVanDon).OrderBy(x => x.Id).Select(x => new Model.Model.BillModel.ListHandling()
                    {
                        MaSoXe = x.MaSoXe,
                        DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemLayTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
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
                        || (y.sfPice.FirstPlace == x.DiemLayTraRong)
                        || (y.sfPice.FirstPlace == z.cd.DiemDau && y.sfPice.SecondPlace == z.cd.DiemCuoi)
                        ).OrderBy(x => x.sfPice).Select(x => new ListSubFeeByContract()
                        {
                            ContractId = x.hd.MaHopDong,
                            ContractName = x.hd.TenHienThi,
                            sfName = x.sf.SfName,
                            goodsType = x.sfPice.GoodsType,
                            firstPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfPice.FirstPlace).Select(x => x.TenDiaDiem).FirstOrDefault(),
                            secondPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfPice.SecondPlace).Select(x => x.TenDiaDiem).FirstOrDefault(),
                            unitPrice = x.sfPice.UnitPrice
                        }).ToList(),
                        listSubFeeIncurreds = _context.SfeeByTcommand.Where(y => y.IdTcommand == x.Id && y.ApproveStatus == 14).OrderBy(x => x.Id).Select(x => new ListSubFeeIncurred()
                        {
                            Note = x.Note,
                            Price = x.FinalPrice,
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
                                       join cd in _context.CungDuong
                                       on vd.MaCungDuong equals cd.MaCungDuong
                                       orderby vd.ThoiGianHoanThanh
                                       select new { kh, vd, cd };

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

                var getListTransport = await getDataTransport.Where(x => x.vd.MaVanDon == transportId).Select(z => new ListVanDon()
                {
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.cd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.cd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    MaVanDon = z.vd.MaVanDon,
                    MaKh = z.kh.MaKh,
                    TenKh = z.kh.TenKh,
                    LoaiVanDon = z.vd.LoaiVanDon,
                    MaCungDuong = z.cd.MaCungDuong,
                    TenCungDuong = z.cd.TenCungDuong,
                    TongTheTich = z.vd.TongTheTich,
                    TongKhoiLuong = z.vd.TongKhoiLuong,
                    listHandling = getlistHandling.Where(y => y.MaVanDon == z.vd.MaVanDon).OrderBy(x => x.Id).Select(x => new Model.Model.BillModel.ListHandling()
                    {
                        MaSoXe = x.MaSoXe,
                        DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemLayTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
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
                        || (y.sfPice.FirstPlace == x.DiemLayTraRong)
                        || (y.sfPice.FirstPlace == z.cd.DiemDau && y.sfPice.SecondPlace == z.cd.DiemCuoi)
                        ).OrderBy(x => x.sfPice).Select(x => new ListSubFeeByContract()
                        {
                            ContractId = x.hd.MaHopDong,
                            ContractName = x.hd.TenHienThi,
                            sfName = x.sf.SfName,
                            goodsType = x.sfPice.GoodsType,
                            firstPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfPice.FirstPlace).Select(x => x.TenDiaDiem).FirstOrDefault(),
                            secondPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfPice.SecondPlace).Select(x => x.TenDiaDiem).FirstOrDefault(),
                            unitPrice = x.sfPice.UnitPrice
                        }).ToList(),
                        listSubFeeIncurreds = _context.SfeeByTcommand.Where(y => y.IdTcommand == x.Id && y.ApproveStatus == 14).OrderBy(x => x.Id).Select(x => new ListSubFeeIncurred()
                        {
                            Note = x.Note,
                            Price = x.FinalPrice,
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
                           join cd in _context.CungDuong
                           on vd.MaCungDuong equals cd.MaCungDuong
                           where getlistHandling.Select(x => x.MaVanDon).Contains(vd.MaVanDon)
                           orderby vd.ThoiGianHoanThanh
                           select new { kh, vd, cd };

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
                MaCungDuong = x.vd.MaCungDuong,
                TenCungDuong = x.cd.TenCungDuong,
                DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.cd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.cd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                TongTheTich = x.vd.TongTheTich,
                TongKhoiLuong = x.vd.TongKhoiLuong
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
                                  join cd in _context.CungDuong
                                  on vd.MaCungDuong equals cd.MaCungDuong
                                  where dp.TrangThai == 20
                                  select new { dp, vd, cd };

            var getListSubFeeByContract = from kh in _context.KhachHang
                                          join hd in _context.HopDongVaPhuLuc
                                          on kh.MaKh equals hd.MaKh
                                          join sfPice in _context.SubFeePrice
                                          on hd.MaHopDong equals sfPice.ContractId
                                          join sf in _context.SubFee
                                          on sfPice.SfId equals sf.SubFeeId
                                          where sfPice.Status == 14
                                          select new { kh, hd, sfPice, sf };

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                getlistHandling = getlistHandling.Where(x => x.dp.ThoiGianHoanThanh.Value >= filter.fromDate.Value && x.dp.ThoiGianHoanThanh.Value <= filter.toDate.Value);
            }

            if (!string.IsNullOrEmpty(filter.customerId))
            {
                getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == filter.customerId || x.vd.MaKh == filter.customerId);
            }

            var totalCount = await getlistHandling.CountAsync();

            var pagedData = await getlistHandling.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListBillHandling()
            {
                MaChuyen = x.dp.Id,
                MaVanDon = x.dp.MaVanDon,
                LoaiHangHoa = x.dp.MaLoaiHangHoa,
                LoaiPhuongTien = x.dp.MaLoaiPhuongTien,
                MaNcc = x.dp.DonViVanTai,
                MaKh = x.vd.MaKh,
                KhachHang = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
                DonGiaKH = x.dp.DonGiaKh.Value,
                DonGiaNCC = x.dp.DonGiaNcc.Value,
                LoiNhuan = x.dp.DonGiaNcc.Value - x.dp.DonGiaKh.Value,
                ChiPhiHopDong = (decimal)getListSubFeeByContract.Where(y => y.kh.MaKh == x.vd.MaKh &&
                ((y.sfPice.GoodsType == x.dp.MaLoaiHangHoa)
                        || (y.sfPice.FirstPlace == x.dp.DiemLayTraRong)
                        || (y.sfPice.FirstPlace == x.cd.DiemDau && y.sfPice.SecondPlace == x.cd.DiemCuoi))
                ).Sum(y => y.sfPice.UnitPrice),
                ChiPhiPhatSinh = ((decimal)_context.SfeeByTcommand.Where(y => y.IdTcommand == x.dp.Id && y.ApproveStatus == 14).Sum(y => y.FinalPrice)),
            }).Select(x => new ListBillHandling()
            {
                MaVanDon = x.MaVanDon,
                MaChuyen = x.MaChuyen,
                LoaiHangHoa = x.LoaiHangHoa,
                LoaiPhuongTien = x.LoaiPhuongTien,
                MaNcc = x.MaNcc,
                MaKh = x.MaKh,
                KhachHang = x.KhachHang,
                DonViVanTai = x.DonViVanTai,
                DonGiaKH = x.DonGiaKH,
                DonGiaNCC = x.DonGiaNCC,
                DoanhThu = x.DonGiaKH.Value + x.ChiPhiPhatSinh + x.ChiPhiHopDong,
                LoiNhuan=x.LoiNhuan,
                ChiPhiHopDong = x.ChiPhiHopDong,
                ChiPhiPhatSinh = x.ChiPhiPhatSinh,
            }).OrderByDescending(x => x.MaChuyen).ToListAsync();

            return new PagedResponseCustom<ListBillHandling>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };

        }
    }
}
