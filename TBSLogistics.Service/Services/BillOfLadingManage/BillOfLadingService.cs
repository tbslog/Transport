using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.BillOfLadingManage
{
    public class BillOfLadingService : IBillOfLading
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;

        public BillOfLadingService(ICommon common, TMSContext context)
        {
            _common = common;
            _context = context;
        }

        public async Task<LoadDataTransPort> getListRoadBillOfLading(string RoadId)
        {
            var getListRoad = from cd in _context.CungDuong
                              join bg in _context.BangGia
                              on cd.MaCungDuong equals bg.MaCungDuong
                              join hd in _context.HopDongVaPhuLuc
                              on bg.MaHopDong equals hd.MaHopDong
                              join kh in _context.KhachHang
                              on hd.MaKh equals kh.MaKh
                              where
                                 cd.TrangThai == 1 &&
                                 bg.NgayApDung.Date <= DateTime.Now.Date
                                 && bg.NgayHetHieuLuc.Date >= DateTime.Now.Date
                                 && bg.TrangThai == 4
                                 && cd.MaCungDuong == RoadId
                              orderby bg.Id descending
                              select new { cd, bg, hd, kh };

            var gr = from t in getListRoad
                     group t by new { t.bg.MaCungDuong, t.bg.MaDvt, t.bg.MaLoaiHangHoa, t.bg.MaLoaiPhuongTien, t.bg.MaPtvc, t.bg.MaLoaiDoiTac, t.kh.MaKh }
                    into g
                     select new
                     {
                         MaCungDuong = g.Key.MaCungDuong,
                         MaDvt = g.Key.MaDvt,
                         MaLoaiHangHoa = g.Key.MaLoaiHangHoa,
                         MaLoaiPhuongTien = g.Key.MaLoaiPhuongTien,
                         MaPtvc = g.Key.MaPtvc,
                         MaLoaiDoiTac = g.Key.MaLoaiDoiTac,
                         MaKH = g.Key.MaKh,
                         Id = (from t2 in g select t2.bg.Id).Max(),
                     };

            getListRoad = getListRoad.Where(x => gr.Select(y => y.Id).Contains(x.bg.Id));

            var listRomooc = from rm in _context.Romooc join lrm in _context.LoaiRomooc on rm.MaRomooc equals lrm.MaLoaiRomooc select new { rm, lrm };

            var result = new LoadDataTransPort()
            {
                ListNhaPhanPhoi = await getListRoad.Where(x => x.bg.MaLoaiDoiTac == "NCC").GroupBy(x => new { x.kh.MaKh, x.kh.TenKh }).Select(x => new NhaPhanPhoiSelect()
                {
                    MaNPP = x.Key.MaKh,
                    TenNPP = x.Key.TenKh
                }).ToListAsync(),
                ListKhachHang = await getListRoad.Where(x => x.bg.MaLoaiDoiTac == "KH").GroupBy(x => new { x.kh.MaKh, x.kh.TenKh }).Select(x => new KhachHangSelect()
                {
                    MaKH = x.Key.MaKh,
                    TenKH = x.Key.TenKh
                }).ToListAsync(),
                BangGiaVanDon = await getListRoad.Select(x => new BangGiaVanDon
                {
                    MaDoiTac = x.kh.MaKh,
                    PhanLoaiDoiTac = x.kh.MaLoaiKh,
                    PTVC = x.bg.MaPtvc,
                    DVT = x.bg.MaDvt,
                    PTVanChuyen = x.bg.MaLoaiPhuongTien,
                    LoaiHangHoa = x.bg.MaLoaiHangHoa,
                    Price = x.bg.DonGia,
                    MaCungDuong = x.bg.MaCungDuong
                }).ToListAsync(),
                ListTaiXe = await _context.TaiXe.Select(x => new DriverTransport()
                {
                    MaTaiXe = x.MaTaiXe,
                    TenTaiXe = x.HoVaTen,
                }).ToListAsync(),
                ListXeVanChuyen = await _context.XeVanChuyen.Select(x => new VehicleTransport()
                {
                    MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                    MaSoXe = x.MaSoXe
                }).ToListAsync(),
                ListRomooc = await listRomooc.Select(x => new RomoocTransport() {
                    MaRomooc = x.rm.MaRomooc,
                    TenLoaiRomooc = x.lrm.TenLoaiRomooc
                }).ToListAsync()
            };
            return result;
        }

        public async Task<BoolActionResult> CreateTransPort(CreateTransport request)
        {
            try
            {
                var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();

                if (checkRoad == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cung đường không tồn tại" };
                }


                string errors = "";
                foreach (var item in request.DieuPhoi)
                {

                }

                var getMaxTransportID = _context.VanDon.Max(x => x.MaVanDon).Select(x => x).FirstOrDefault().ToString();
                string transPortId = "";

                if (string.IsNullOrEmpty(getMaxTransportID))
                {
                    transPortId = DateTime.Now.ToString("yy") + "000000001";
                }
                else
                {
                    transPortId = DateTime.Now.ToString("yy") + (int.Parse(getMaxTransportID.Substring(2, getMaxTransportID.Length)) + 1).ToString("000000000");
                }


                var createTransport = _context.VanDon.Add(new VanDon()
                {
                    MaVanDon = transPortId,
                    MaCungDuong = request.MaCungDuong,
                    TrangThai = 8,
                    NgayTaoDon = DateTime.Now.Date,
                    UpdatedTime = DateTime.Now,
                    CreatedTime = DateTime.Now
                });

                await _context.SaveChangesAsync();


                var handling = request.DieuPhoi.Select(x => new DieuPhoi()
                {
                    MaVanDon = createTransport.Entity.MaVanDon,
                    MaSoXe = x.MaSoXe,
                    MaTaiXe = x.MaTaiXe,
                    DonViVanTai = x.DonViVanTai,
                    MaKh = x.MaKh,
                    IdbangGia = x.IdbangGia,
                    GiaThamChieu = x.GiaThamChieu,
                    GiaThucTe = x.GiaThucTe,
                    MaRomooc = x.MaRomooc,
                    ContNo = x.ContNo,
                    SealNp = x.SealNp,
                    SealHq = x.SealHq,
                    TrongLuong = x.TrongLuong,
                    TheTich = x.TheTich,
                    GhiChu = x.GhiChu,
                    ThoiGianLayRong = x.ThoiGianLayRong,
                    ThoiGianHaCong = x.ThoiGianHaCong,
                    ThoiGianKeoCong = x.ThoiGianKeoCong,
                    ThoiGianHanLech = x.ThoiGianHanLech,
                    ThoiGianCoMat = x.ThoiGianCoMat,
                    ThoiGianCatMang = x.ThoiGianCatMang,
                    ThoiGianTraRong = x.ThoiGianTraRong,
                    ThoiGianNhapHang = x.ThoiGianNhapHang,
                    ThoiGianXaHang = x.ThoiGianXaHang,
                    TrangThai = 8,
                    CreatedTime = DateTime.Now,
                }).ToList();
                await _context.DieuPhoi.AddRangeAsync(handling);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }
    }
}
