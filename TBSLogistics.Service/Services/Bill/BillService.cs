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
        public async Task<PagedResponseCustom<ListCustomerHasBill>> GetListCustomerHasBill(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);


            if (string.IsNullOrEmpty(filter.fromDate.ToString()) && string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                return null;
            }

            var listData = from kh in _context.KhachHang
                           join vd in _context.VanDon
                           on kh.MaKh equals vd.MaKh
                           join dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon
                           where dp.TrangThai == 20
                            && dp.ThoiGianHoanThanh.Value.Date >= filter.fromDate.Value.Date
                            && dp.ThoiGianHoanThanh.Value.Date <= filter.toDate.Value.Date
                           select new { kh, vd, dp };

            var listKh = from kh in _context.KhachHang
                         where listData.Select(x => x.kh.MaKh).Contains(kh.MaKh)
                         select kh;

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.vd.MaKh.Contains(filter.Keyword) || x.kh.TenKh.Contains(filter.Keyword));
            }

            var totalCount = await listKh.CountAsync();

            var pagedData = await listKh.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListCustomerHasBill()
            {
                MaKh = x.MaKh,
                TenKh = x.TenKh,
                SoDienThoai = x.Sdt,
                Email = x.Email,
                TongVanDon = _context.VanDon.Where(y => listData.Select(a => a.vd.MaVanDon).Contains(y.MaVanDon)).Count(),
                TongSoChuyen = _context.DieuPhoi.Where(y => listData.Select(a => a.dp.Id).Contains(y.Id)).Count(),
                //TongPhuPhiPhatSinh = _context.SfeeByTcommand.Where(y => listData.Select(a => a.dp.Id).Contains(y.IdTcommand) && y.ApproveStatus == 14).Count(),
                //TongPhiPhiTheoHopDong =  ,
            }).OrderBy(x => x.MaKh).ToListAsync();

            return new PagedResponseCustom<ListCustomerHasBill>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<GetBill> GetBillByCustomerId(string customerId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var checkExists = await _context.KhachHang.Where(x => x.MaKh == customerId).FirstOrDefaultAsync();

                if (checkExists == null)
                {
                    return null;
                }

                var getlistHandling = from dp in _context.DieuPhoi
                                      where dp.TrangThai == 20
                                      && dp.ThoiGianHoanThanh.Value.Date >= fromDate
                                      && dp.ThoiGianHoanThanh.Value.Date <= toDate
                                      select dp;

                var getDataTransport = from kh in _context.KhachHang
                                       join vd in _context.VanDon
                                       on kh.MaKh equals vd.MaKh
                                       join cd in _context.CungDuong
                                       on vd.MaCungDuong equals cd.MaCungDuong
                                       where vd.MaKh == customerId
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
                    listHandling = getlistHandling.Where(y => y.MaVanDon == z.vd.MaVanDon).OrderBy(x => x.Id).Select(x => new ListHandling()
                    {
                        MaSoXe = x.MaSoXe,
                        DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemLayTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                        MaRomooc = x.MaRomooc,
                        TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
                        LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
                        LoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
                        DonViTinh = _context.DonViTinh.Where(y => y.MaDvt == x.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
                        DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.DonViVanTai).Select(x => x.TenKh).FirstOrDefault(),
                        DonGia = x.GiaThamChieu,
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
    }
}
