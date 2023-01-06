using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.BillOfLadingManage
{
    public class BillOfLadingService : IBillOfLading
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public BillOfLadingService(ICommon common, TMSContext context, IHttpContextAccessor httpContextAccessor)
        {
            _common = common;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }

        public async Task<ListPoint> LoadDataRoadTransportByCusId(string customerId)
        {
            var checkCus = await _context.KhachHang.Where(x => x.MaKh == customerId).FirstOrDefaultAsync();

            if (checkCus == null)
            {
                return null;
            }

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
                                 && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                                 && bg.TrangThai == 4
                                 && kh.MaKh == customerId
                              select new { cd };

            var gr = from t in getListRoad
                     group t by new { t.cd.MaCungDuong }
                   into g
                     select new
                     {
                         g.Key.MaCungDuong,
                     };

            var listRoad = await _context.CungDuong.Where(x => gr.Select(y => y.MaCungDuong).Contains(x.MaCungDuong)).ToListAsync();

            return new ListPoint()
            {
                DiemDau = listRoad.Select(x => x.DiemDau).Select(x => new Point()
                {
                    MaDiaDiem = x,
                    TenDiaDiem = _context.DiaDiem.Where(y => y.MaDiaDiem == x).Select(y => y.TenDiaDiem).FirstOrDefault()
                }).ToList(),
                DiemCuoi = listRoad.Select(x => x.DiemCuoi).Select(x => new Point()
                {
                    MaDiaDiem = x,
                    TenDiaDiem = _context.DiaDiem.Where(y => y.MaDiaDiem == x).Select(y => y.TenDiaDiem).FirstOrDefault()
                }).ToList(),
                CungDuong = listRoad.Select(x => new Road()
                {
                    DiemCuoi = x.DiemCuoi,
                    DiemDau = x.DiemDau,
                    MaCungDuong = x.MaCungDuong,
                    TenCungDuong = x.TenCungDuong,
                    KM = x.Km,
                }).ToList()
            };
        }

        public async Task<LoadDataHandling> LoadDataHandling()
        {
            var listRomooc = from rm in _context.Romooc join lrm in _context.LoaiRomooc on rm.MaLoaiRomooc equals lrm.MaLoaiRomooc select new { rm, lrm };

            var result = new LoadDataHandling()
            {
                ListNhaPhanPhoi = await _context.KhachHang.Where(x => x.MaLoaiKh == "NCC").GroupBy(x => new { x.MaKh, x.TenKh }).Select(x => new NhaPhanPhoiSelect()
                {
                    MaNPP = x.Key.MaKh,
                    TenNPP = x.Key.TenKh
                }).ToListAsync(),
                ListKhachHang = await _context.KhachHang.Where(x => x.MaLoaiKh == "KH").GroupBy(x => new { x.MaKh, x.TenKh }).Select(x => new KhachHangSelect()
                {
                    MaKH = x.Key.MaKh,
                    TenKH = x.Key.TenKh
                }).ToListAsync(),
                ListTaiXe = await _context.TaiXe.Select(x => new DriverTransport()
                {
                    MaTaiXe = x.MaTaiXe,
                    TenTaiXe = x.HoVaTen,
                }).ToListAsync(),
                ListXeVanChuyen = await _context.XeVanChuyen.Select(x => new VehicleTransport()
                {
                    MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                    MaSoXe = x.MaSoXe,
                }).ToListAsync(),
                ListRomooc = await listRomooc.Select(x => new RomoocTransport()
                {
                    MaRomooc = x.rm.MaRomooc,
                    TenLoaiRomooc = x.lrm.TenLoaiRomooc
                }).ToListAsync()
            };
            return result;
        }

        public async Task<BoolActionResult> CreateHandling(CreateHandling request)
        {
            try
            {
                var transport = await _context.VanDon.Where(x => x.MaVanDon == request.MaVanDon).FirstOrDefaultAsync();
                if (transport == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn không tồn tại" };
                }

                var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();
                if (checkRoad == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cung đường không tồn tại" };
                }

                string ErrorValidate = "";

                if (!string.IsNullOrEmpty(ErrorValidate))
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                using var transaction = _context.Database.BeginTransaction();

                transport.TrangThai = 9;
                _context.VanDon.Update(transport);

                string MaChuyen = transport.MaPtvc.Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");

                if (transport.MaPtvc == "LCL" || transport.MaPtvc == "LTL")
                {
                    var getHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == transport.MaVanDon).FirstOrDefaultAsync();

                    if (getHandling != null)
                    {
                        MaChuyen = getHandling.MaChuyen;
                    }
                }

                foreach (var item in request.DieuPhoi)
                {
                    var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == item.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();

                    if (checkVehicleType == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                    }

                    var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == item.LoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                    if (checkGoodsType == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                    }
                    var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == item.DonViTinh).Select(x => x.TenDvt).FirstOrDefaultAsync();
                    if (checkDVT == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                    }
                    var checkPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == item.MaPTVC).Select(x => x.TenPtvc).FirstOrDefaultAsync();
                    if (checkPTVC == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Phương Thức Vận Chuyển Không Tồn Tại" };
                    }

                    if (item.PTVanChuyen.Contains("CONT"))
                    {
                        var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemLayTraRong.Value).FirstOrDefaultAsync();
                        if (checkPlaceGetEmpty == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                        }
                    }

                    var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                    if (checkSupplier == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                    }

                    var checkPriceTable = from bg in _context.BangGia
                                          join hd in _context.HopDongVaPhuLuc
                                          on bg.MaHopDong equals hd.MaHopDong
                                          where (hd.MaKh == item.MaKH || hd.MaKh == item.DonViVanTai)
                                          && bg.MaCungDuong == request.MaCungDuong
                                          && bg.TrangThai == 4
                                          && bg.NgayApDung.Date <= DateTime.Now.Date
                                          && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                                          && bg.MaDvt == item.DonViTinh
                                          && bg.MaLoaiHangHoa == item.LoaiHangHoa
                                          && bg.MaLoaiPhuongTien == item.PTVanChuyen
                                          && bg.MaPtvc == item.MaPTVC
                                          select bg;

                    if (checkPriceTable.Count() == 2)
                    {
                        var priceTableSupplier = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").FirstOrDefaultAsync();
                        if (priceTableSupplier == null)
                        {
                            return new BoolActionResult
                            {
                                isSuccess = false,
                                Message = "Đơn vị vận tải: "
                                + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                            + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                            ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            ", Loại Hàng Hóa:" + checkGoodsType +
                            ", Đơn Vị Tính: " + checkDVT +
                            ", Phương thức vận chuyển: " + checkPTVC
                            };
                        }

                        var priceTableCustomer = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").FirstOrDefaultAsync();
                        if (priceTableCustomer == null)
                        {
                            return new BoolActionResult
                            {
                                isSuccess = false,
                                Message = "Khách Hàng: "
                               + await _context.KhachHang.Where(x => x.MaKh == item.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                            + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                            ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            ", Loại Hàng Hóa:" + checkGoodsType +
                            ", Đơn Vị Tính: " + checkDVT +
                            ", Phương thức vận chuyển: " + checkPTVC
                            };
                        }

                        var itemHandling = new DieuPhoi();
                        itemHandling.MaChuyen = MaChuyen;
                        itemHandling.MaVanDon = transport.MaVanDon;
                        itemHandling.MaLoaiHangHoa = item.LoaiHangHoa;
                        itemHandling.MaLoaiPhuongTien = item.PTVanChuyen;
                        itemHandling.MaDvt = item.DonViTinh;
                        itemHandling.DonViVanTai = item.DonViVanTai;
                        itemHandling.BangGiaKh = priceTableCustomer.Id;
                        itemHandling.BangGiaNcc = priceTableSupplier.Id;
                        itemHandling.DonGiaKh = priceTableCustomer.DonGia;
                        itemHandling.DonGiaNcc = priceTableSupplier.DonGia;
                        itemHandling.KhoiLuong = item.KhoiLuong;
                        itemHandling.TheTich = item.TheTich;
                        itemHandling.SoKien = item.SoKien;
                        itemHandling.DiemLayTraRong = item.DiemLayTraRong;
                        itemHandling.TrangThai = 19;
                        itemHandling.CreatedTime = DateTime.Now;
                        await _context.DieuPhoi.AddAsync(itemHandling);
                    }
                    else
                    {
                        var priceTableSupplier = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").FirstOrDefaultAsync();
                        if (priceTableSupplier == null)
                        {
                            return new BoolActionResult
                            {
                                isSuccess = false,
                                Message = "Đơn vị vận tải: "
                                + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                            + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                            ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            ", Loại Hàng Hóa:" + checkGoodsType +
                            ", Đơn Vị Tính: " + checkDVT +
                            ", Phương thức vận chuyển: " + checkPTVC
                            };
                        }

                        var priceTableCustomer = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").FirstOrDefaultAsync();
                        if (priceTableCustomer == null)
                        {
                            return new BoolActionResult
                            {
                                isSuccess = false,
                                Message = "Khách Hàng: "
                               + await _context.KhachHang.Where(x => x.MaKh == item.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                            + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                            ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            ", Loại Hàng Hóa:" + checkGoodsType +
                            ", Đơn Vị Tính: " + checkDVT +
                            ", Phương thức vận chuyển: " + checkPTVC
                            };
                        }

                        return new BoolActionResult { isSuccess = false, Message = "Không có bảng giá khách hàng lẫn nhà cung cấp" };
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    transaction.Commit();
                    await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " create new BillOfLading with Data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Điều Phố Vận Đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điều Phố Vận Đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " create new BillOfLading with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CreateTransport(CreateTransport request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();
                if (checkRoad == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cung đường không tồn tại" };
                }

                var checkCus = await _context.KhachHang.Where(x => x.MaKh == request.MaKH).FirstOrDefaultAsync();

                if (checkCus == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Khách hàng không tồn tại" };
                }

                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (string.IsNullOrEmpty(request.MaVanDonKH))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng không bỏ trống Mã Vận Đơn Khách Hàng" };
                }

                if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() > 0)
                {
                    //if (request.ThoiGianLayTraRong == null)
                    //{
                    //    return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian lấy/trả rỗng" };
                    //}

                    if (request.LoaiVanDon == "nhap")
                    {
                        if (request.ThoiGianHanLenh == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                        }

                        if (request.ThoiGianLayTraRong != null && request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                        {
                            if (request.ThoiGianLayTraRong <= request.ThoiGianLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }

                            if (request.ThoiGianLayTraRong <= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }
                    }
                    else
                    {
                        if (request.ThoiGianHaCang == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                        }

                        if (request.ThoiGianLayTraRong != null && request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                        {
                            if (request.ThoiGianLayTraRong >= request.ThoiGianLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }
                            if (request.ThoiGianLayTraRong >= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }
                    }
                }

                if (request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                {
                    if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy hàng không được lơn hơn hoặc bằng Thời Gian Trả Hàng" };
                    }
                }

                var getMaxTransportID = await _context.VanDon.OrderByDescending(x => x.MaVanDon).Select(x => x.MaVanDon).FirstOrDefaultAsync();
                string transPortId = "";

                if (string.IsNullOrEmpty(getMaxTransportID))
                {
                    transPortId = DateTime.Now.ToString("yy") + "00000001";
                }
                else
                {
                    transPortId = DateTime.Now.ToString("yy") + (int.Parse(getMaxTransportID.Substring(2, getMaxTransportID.Length - 2)) + 1).ToString("00000000");
                }

                if (request.LoaiVanDon == "nhap")
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() == 0)
                    {
                        request.ThoiGianLayTraRong = null;
                        request.ThoiGianCoMat = null;
                        request.ThoiGianHanLenh = null;
                    }
                    request.HangTau = null;
                    request.TenTau = null;
                }
                else
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() == 0)
                    {
                        request.ThoiGianHaCang = null;
                        request.ThoiGianLayTraRong = null;
                    }
                }

                await _context.VanDon.AddRangeAsync(new VanDon()
                {
                    MaPtvc = request.MaPTVC,
                    TongSoKien = request.TongSoKien,
                    MaKh = request.MaKH,
                    HangTau = request.HangTau,
                    Tau = request.TenTau,
                    MaVanDon = transPortId,
                    MaVanDonKh = request.MaVanDonKH,
                    TongThungHang = request.arrHandlings.Count(),
                    LoaiVanDon = request.LoaiVanDon,
                    MaCungDuong = request.MaCungDuong,
                    TongKhoiLuong = request.TongKhoiLuong,
                    TongTheTich = request.TongTheTich,
                    ThoiGianLayTraRong = request.ThoiGianLayTraRong,
                    ThoiGianCoMat = request.ThoiGianCoMat,
                    ThoiGianHanLenh = request.ThoiGianHanLenh,
                    ThoiGianHaCang = request.ThoiGianHaCang,
                    ThoiGianLayHang = request.ThoiGianLayHang,
                    ThoiGianTraHang = request.ThoiGianTraHang,
                    GhiChu = request.GhiChu,
                    TrangThai = 8,
                    ThoiGianTaoDon = DateTime.Now,
                    CreatedTime = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    foreach (var item in request.arrHandlings)
                    {
                        var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == item.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                        if (checkVehicleType == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                        }

                        var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == item.LoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                        if (checkGoodsType == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                        }

                        var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == item.DonViTinh).Select(x => x.TenDvt).FirstOrDefaultAsync();
                        if (checkDVT == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                        }

                        var checkPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == request.MaPTVC).Select(x => x.TenPtvc).FirstOrDefaultAsync();
                        if (checkPTVC == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Phương Thức Vận Chuyển Không Tồn Tại" };
                        }

                        if (item.PTVanChuyen.Contains("CONT"))
                        {
                            var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemLayTraRong.Value).FirstOrDefaultAsync();
                            if (checkPlaceGetEmpty == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                            }
                        }

                        var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                        if (checkSupplier == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                        }

                        var priceSup = await GetPriceTable(item.DonViVanTai, request.MaCungDuong, item.DonViTinh, item.LoaiHangHoa, item.PTVanChuyen, request.MaPTVC);
                        var priceCus = await GetPriceTable(request.MaKH, request.MaCungDuong, item.DonViTinh, item.LoaiHangHoa, item.PTVanChuyen, request.MaPTVC);

                        if (priceSup == null)
                        {
                            return new BoolActionResult
                            {
                                isSuccess = false,
                                Message = "Đơn vị vận tải: "
                                + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                            + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                            ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            ", Loại Hàng Hóa:" + checkGoodsType +
                            ", Đơn Vị Tính: " + checkDVT +
                            ", Phương thức vận chuyển: " + checkPTVC
                            };
                        }

                        if (priceCus == null)
                        {
                            return new BoolActionResult
                            {
                                isSuccess = false,
                                Message = "Khách Hàng: "
                               + await _context.KhachHang.Where(x => x.MaKh == request.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                            + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                            ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            ", Loại Hàng Hóa:" + checkGoodsType +
                            ", Đơn Vị Tính: " + checkDVT +
                            ", Phương thức vận chuyển: " + checkPTVC
                            };
                        }

                        var itemHandling = new DieuPhoi();
                        itemHandling.MaChuyen = request.MaPTVC.Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        itemHandling.MaVanDon = transPortId;
                        itemHandling.MaLoaiHangHoa = item.LoaiHangHoa;
                        itemHandling.MaLoaiPhuongTien = item.PTVanChuyen;
                        itemHandling.MaDvt = item.DonViTinh;
                        itemHandling.DonViVanTai = item.DonViVanTai;
                        itemHandling.BangGiaKh = priceCus.ID;
                        itemHandling.BangGiaNcc = priceSup.ID;
                        itemHandling.DonGiaKh = priceCus.DonGia;
                        itemHandling.DonGiaNcc = priceSup.DonGia;
                        itemHandling.KhoiLuong = item.KhoiLuong;
                        itemHandling.TheTich = item.TheTich;
                        itemHandling.SoKien = item.SoKien;
                        itemHandling.DiemLayTraRong = item.DiemLayTraRong;
                        itemHandling.TrangThai = 19;
                        itemHandling.CreatedTime = DateTime.Now;
                        await _context.DieuPhoi.AddAsync(itemHandling);
                    }

                    var resultDP = await _context.SaveChangesAsync();

                    if (resultDP > 0)
                    {
                        await transaction.CommitAsync();
                        await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " create new Transport with Data: " + JsonSerializer.Serialize(request));
                        return new BoolActionResult { isSuccess = true, Message = "Tạo vận đơn Thành Công!" };
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                    }
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + " create new Transport with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CreateTransportLess(CreateTransportLess request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var checkTransportType = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == request.MaPTVC).FirstOrDefaultAsync();

                if (checkTransportType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Phương thức vận chuyển không tồn tại" };
                }

                var checkCustomer = await _context.KhachHang.Where(x => x.MaKh == request.MaKH && x.MaLoaiKh == "KH").FirstOrDefaultAsync();
                if (checkCustomer == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Khách hàng không tồn tại" };
                }

                var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();
                if (checkRoad == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã cung đường không yồn tại" };
                }

                //if (request.TongKhoiLuong < 1 || request.TongTheTich < 1 || request.TongSoKien < 1)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Tổng khối lượng, tổng thể tích, tổng thùng hàng không được nhỏ hơn 1" };
                //}

                if (request.ThoiGianLayHang != null || request.ThoiGianLayHang != null)
                {
                    if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Mốc thời gian không đúng" };
                    }
                }

                var getMaxTransportID = await _context.VanDon.OrderByDescending(x => x.MaVanDon).Select(x => x.MaVanDon).FirstOrDefaultAsync();
                string transPortId = "";

                if (string.IsNullOrEmpty(getMaxTransportID))
                {
                    transPortId = DateTime.Now.ToString("yy") + "00000001";
                }
                else
                {
                    transPortId = DateTime.Now.ToString("yy") + (int.Parse(getMaxTransportID.Substring(2, getMaxTransportID.Length - 2)) + 1).ToString("00000000");
                }

                await _context.VanDon.AddRangeAsync(new VanDon()
                {
                    MaVanDon = transPortId,
                    Tau = request.TenTau,
                    HangTau = request.HangTau,
                    LoaiVanDon = request.LoaiVanDon,
                    MaPtvc = request.MaPTVC,
                    MaKh = request.MaKH,
                    MaVanDonKh = request.MaVanDonKH,
                    MaCungDuong = request.MaCungDuong,
                    TongKhoiLuong = request.TongKhoiLuong,
                    TongSoKien = request.TongSoKien,
                    TongTheTich = request.TongTheTich,
                    TongThungHang = 1,
                    ThoiGianTraHang = request.ThoiGianTraHang,
                    ThoiGianLayHang = request.ThoiGianLayHang,
                    GhiChu = request.GhiChu,
                    TrangThai = 8,
                    ThoiGianTaoDon = DateTime.Now,
                    CreatedTime = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Tạo vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<LoadJoinTransports> LoadJoinTransport(JoinTransports request)
        {
            try
            {
                var list = from vd in _context.VanDon
                           join dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon into vdless
                           from vdl in vdless.DefaultIfEmpty()
                           select new { vd, dpl = vdl == null ? null : vdl };

                if (!string.IsNullOrEmpty(request.MaVanDonChung) && request.TransportIds.Count == 0)
                {
                    list = list.Where(x => x.dpl.MaChuyen == request.MaVanDonChung);
                }
                else
                {
                    list = list.Where(x => request.TransportIds.Contains(x.vd.MaVanDon) && x.vd.TrangThai == 8);

                    if (list.Count() != request.TransportIds.Count)
                    {
                        return new LoadJoinTransports { MessageErrors = "Vui lòng chỉ chọn vận đơn có trạng thái 'Chờ Điều Phối' " };
                    }

                    if (list.Where(x => x.vd.LoaiVanDon == list.Select(y => y.vd.LoaiVanDon).FirstOrDefault()).Count() != request.TransportIds.Count)
                    {
                        return new LoadJoinTransports { MessageErrors = "Vui lòng chỉ chọn vận đơn có cùng loại vận đơn (Nhập/Xuất)" };
                    }

                    if (list.Where(x => x.vd.MaPtvc == list.Select(x => x.vd.MaPtvc).FirstOrDefault()).Count() != request.TransportIds.Count)
                    {
                        return new LoadJoinTransports { MessageErrors = "Chỉ được ghép các vận đơn có cùng phương thức vận chuyển (LCL/LTL) " };
                    }
                }

                var data = await list.ToListAsync();

                return new LoadJoinTransports()
                {
                    LoaiVanDon = data.Select(x => x.vd.LoaiVanDon).FirstOrDefault(),
                    HangTau = data.Select(x => x.vd.HangTau).FirstOrDefault(),
                    TenTau = data.Select(x => x.vd.Tau).FirstOrDefault(),
                    MaPTVC = data.Select(x => x.vd.MaPtvc).FirstOrDefault(),
                    handlingLess = data.Select(x => x.dpl).FirstOrDefault() != null ? data.Select(x => new CreateHandlingLess()
                    {
                        PTVanChuyen = x.dpl.MaLoaiPhuongTien,
                        DiemLayTraRong = x.dpl.DiemLayTraRong,
                        DonViVanTai = x.dpl.DonViVanTai,
                        XeVanChuyen = x.dpl.MaSoXe,
                        TaiXe = x.dpl.MaTaiXe,
                        GhiChu = x.dpl.GhiChu,
                        Romooc = x.dpl.MaRomooc,
                        CONTNO = x.dpl.ContNo,
                        SEALHQ = x.dpl.SealHq,
                        SEALNP = x.dpl.SealNp,
                        TGLayTraRong = x.vd.ThoiGianLayTraRong,
                        TGHanLenh = x.vd.ThoiGianHanLenh,
                        TGHaCang = x.vd.ThoiGianHaCang,
                    }).FirstOrDefault() : null,
                    loadTransports = data.Select(x => new LoadTransports()
                    {
                        MaKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                        MaVanDon = x.vd.MaVanDon,
                        MaVanDonKH = x.vd.MaVanDonKh,
                        CungDuong = _context.CungDuong.Where(y => y.MaCungDuong == x.vd.MaCungDuong).Select(y => y.TenCungDuong).FirstOrDefault(),
                        DiemDau = _context.DiaDiem.Where(z => z.MaDiaDiem == _context.CungDuong.Where(y => y.MaCungDuong == x.vd.MaCungDuong).Select(y => y.DiemDau).FirstOrDefault()).Select(z => z.TenDiaDiem).FirstOrDefault(),
                        DiemCuoi = _context.DiaDiem.Where(z => z.MaDiaDiem == _context.CungDuong.Where(y => y.MaCungDuong == x.vd.MaCungDuong).Select(y => y.DiemCuoi).FirstOrDefault()).Select(z => z.TenDiaDiem).FirstOrDefault(),
                        TongKhoiLuong = x.vd.TongKhoiLuong,
                        TongTheTich = x.vd.TongTheTich,
                        TongSoKien = x.vd.TongSoKien,
                        ThoiGianLayHang = x.vd.ThoiGianLayHang,
                        ThoiGianTraHang = x.vd.ThoiGianTraHang,
                    }).ToList(),
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BoolActionResult> CreateHandlingLess(CreateHandlingLess request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == request.XeVanChuyen).FirstOrDefaultAsync();

                //if (checkVehicle == null)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Xe này không tồn tại trong hệ thống" };
                //}

                var loadTransports = await _context.VanDon.Where(x => request.arrTransports.Select(x => x.MaVanDon).Contains(x.MaVanDon) && x.TrangThai == 8).ToListAsync();
                if (loadTransports.Count != request.arrTransports.Count())
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có trạng thái 'Chờ Điều Phối' " };
                }

                if (loadTransports.Where(x => x.LoaiVanDon == loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault()).Count() != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có cùng loại vận đơn (Nhập/Xuất)" };
                }

                if (loadTransports.Where(x => x.MaPtvc == loadTransports.Select(x => x.MaPtvc).FirstOrDefault()).Count() != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Chỉ được ghép các vận đơn có cùng phương thức vận chuyển(LCL/LTL) " };
                }

                if (loadTransports.Count != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Dữ liệu input không hợp lệ" };
                }

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                if (checkVehicleType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                }

                if (loadTransports.Select(x => x.MaPtvc).FirstOrDefault().Contains("LCL"))
                {
                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.DiemLayTraRong.Value).FirstOrDefaultAsync();
                    if (checkPlaceGetEmpty == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                    }
                }

                var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                if (checkSupplier == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                }

                string MaVanDonChung = loadTransports.Select(x => x.MaPtvc).FirstOrDefault().Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");

                foreach (var item in loadTransports)
                {
                    foreach (var itemRequest in request.arrTransports)
                    {
                        if (itemRequest.MaVanDon == item.MaVanDon)
                        {
                            var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == itemRequest.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                            if (checkGoodsType == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                            }

                            var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == itemRequest.MaDVT).Select(x => x.TenDvt).FirstOrDefaultAsync();
                            if (checkDVT == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                            }

                            var priceTableCus = await GetPriceTable(item.MaKh, item.MaCungDuong, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);
                            var priceTableSup = await GetPriceTable(request.DonViVanTai, item.MaCungDuong, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);

                            if (priceTableSup == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + item.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.MaPtvc
                                };
                            }

                            if (priceTableCus == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Khách Hàng: "
                                   + await _context.KhachHang.Where(x => x.MaKh == item.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + item.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.MaPtvc
                                };
                            }

                            if (item.MaPtvc == "LTL")
                            {
                                item.ThoiGianLayTraRong = null;
                                item.ThoiGianHaCang = null;
                                item.ThoiGianHanLenh = null;
                                request.DiemLayTraRong = null;
                                request.Romooc = null;
                            }

                            if (item.MaPtvc == "LCL")
                            {
                                item.ThoiGianLayTraRong = request.TGLayTraRong;

                                if (item.LoaiVanDon == "nhap")
                                {
                                    item.ThoiGianHanLenh = request.TGHanLenh;
                                    item.ThoiGianHaCang = null;
                                }

                                if (item.LoaiVanDon == "xuat")
                                {
                                    item.ThoiGianHanLenh = null;
                                    item.ThoiGianHaCang = request.TGHaCang;
                                }
                            }

                            int trangthai = 19;
                            if (!string.IsNullOrEmpty(request.XeVanChuyen) && !string.IsNullOrEmpty(request.TaiXe))
                            {
                                item.TrangThai = 9;
                                trangthai = 27;
                            }
                            _context.VanDon.Update(item);

                            await _context.DieuPhoi.AddAsync(new DieuPhoi()
                            {
                                MaVanDon = item.MaVanDon,
                                MaChuyen = MaVanDonChung,
                                MaSoXe = request.XeVanChuyen,
                                MaTaiXe = request.TaiXe,
                                MaLoaiHangHoa = itemRequest.MaLoaiHangHoa,
                                MaLoaiPhuongTien = request.PTVanChuyen,
                                MaDvt = itemRequest.MaDVT,
                                DonViVanTai = request.DonViVanTai,
                                BangGiaKh = priceTableCus.ID,
                                BangGiaNcc = priceTableSup.ID,
                                DonGiaKh = priceTableCus.DonGia,
                                DonGiaNcc = priceTableSup.DonGia,
                                MaRomooc = request.Romooc,
                                ContNo = request.CONTNO,
                                SealNp = request.SEALNP,
                                SealHq = request.SEALHQ,
                                SoKien = item.TongSoKien,
                                KhoiLuong = item.TongKhoiLuong,
                                TheTich = item.TongTheTich,
                                GhiChu = item.GhiChu,
                                DiemLayTraRong = request.DiemLayTraRong,
                                TrangThai = trangthai,
                                CreatedTime = DateTime.Now,
                            });
                        }
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Ghép vận đơn với xe thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Ghép vận đơn với xe thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            };
        }

        public async Task<BoolActionResult> UpdateHandlingLess(string handlingId, UpdateHandlingLess request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == request.XeVanChuyen).FirstOrDefaultAsync();

                if (checkVehicle == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Xe này không tồn tại trong hệ thống" };
                }

                var loadTransports = await _context.VanDon.Where(x => request.arrTransports.Select(x => x.MaVanDon).Contains(x.MaVanDon)).ToListAsync();
                //if (loadTransports.Count != request.arrTransports.Count())
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có trạng thái 'Chờ Điều Phối' " };
                //}

                if (loadTransports.Where(x => x.LoaiVanDon == loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault()).Count() != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có cùng loại vận đơn (Nhập/Xuất)" };
                }

                if (loadTransports.Where(x => x.MaPtvc == loadTransports.Select(x => x.MaPtvc).FirstOrDefault()).Count() != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Chỉ được ghép các vận đơn có cùng phương thức vận chuyển(LCL/LTL) " };
                }

                if (loadTransports.Count != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Dữ liệu input không hợp lệ" };
                }

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                if (checkVehicleType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                }

                if (loadTransports.Select(x => x.MaPtvc).FirstOrDefault().Contains("LCL"))
                {
                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.DiemLayTraRong.Value).FirstOrDefaultAsync();
                    if (checkPlaceGetEmpty == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                    }
                }

                var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                if (checkSupplier == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                }

                var data = from vd in _context.VanDon
                           join dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon
                           where dp.MaChuyen == handlingId
                           select new { vd, dp };

                var dataRemove = await data.Where(x => !request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToListAsync();
                _context.DieuPhoi.RemoveRange(dataRemove.Select(x => x.dp));
                dataRemove.ForEach(x =>
                {
                    x.vd.TrangThai = 8;
                    x.vd.UpdatedTime = DateTime.Now;
                });

                var listTransport = await data.Where(x => request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToListAsync();

                foreach (var x in listTransport.Select(x => x.vd).ToList())
                {
                    if (x.TrangThai == 11 || x.TrangThai == 22)
                    {
                        return new BoolActionResult() { isSuccess = false, Message = "Không thể cập nhật chuyến này nữa" };
                    }

                    x.UpdatedTime = DateTime.Now;

                    if (!string.IsNullOrEmpty(request.XeVanChuyen) && !string.IsNullOrEmpty(request.TaiXe) && x.TrangThai == 8)
                    {
                        x.TrangThai = 9;
                    }

                    if (x.MaPtvc == "LTL")
                    {
                        x.ThoiGianLayTraRong = null;
                        x.ThoiGianHaCang = null;
                        x.ThoiGianHanLenh = null;
                        request.DiemLayTraRong = null;
                        request.Romooc = null;
                    }

                    if (x.MaPtvc == "LCL")
                    {
                        x.ThoiGianLayTraRong = request.TGLayTraRong;

                        if (x.LoaiVanDon == "nhap")
                        {
                            x.ThoiGianHanLenh = request.TGHanLenh;
                            x.ThoiGianHaCang = null;
                        }

                        if (x.LoaiVanDon == "xuat")
                        {
                            x.ThoiGianHanLenh = null;
                            x.ThoiGianHaCang = request.TGHaCang;
                        }
                    }
                    _context.VanDon.Update(x);
                }

                foreach (var item in data.Where(x => request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToList())
                {
                    foreach (var itemRequest in request.arrTransports)
                    {
                        if (item.dp.MaVanDon == itemRequest.MaVanDon)
                        {
                            var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == itemRequest.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                            if (checkGoodsType == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                            }

                            var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == itemRequest.MaDVT).Select(x => x.TenDvt).FirstOrDefaultAsync();
                            if (checkDVT == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                            }

                            var priceTableCus = await GetPriceTable(item.vd.MaKh, item.vd.MaCungDuong, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.vd.MaPtvc);
                            var priceTableSup = await GetPriceTable(request.DonViVanTai, item.vd.MaCungDuong, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.vd.MaPtvc);

                            if (priceTableSup == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + item.vd.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.vd.MaPtvc
                                };
                            }

                            if (priceTableCus == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Khách Hàng: "
                                   + await _context.KhachHang.Where(x => x.MaKh == item.vd.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + item.vd.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.vd.MaPtvc
                                };
                            }

                            item.dp.MaSoXe = request.XeVanChuyen;
                            item.dp.MaTaiXe = request.TaiXe;
                            item.dp.MaLoaiHangHoa = itemRequest.MaLoaiHangHoa;
                            item.dp.MaLoaiPhuongTien = request.PTVanChuyen;
                            item.dp.MaDvt = itemRequest.MaDVT;
                            item.dp.DonViVanTai = request.DonViVanTai;
                            item.dp.BangGiaKh = priceTableCus.ID;
                            item.dp.BangGiaNcc = priceTableSup.ID;
                            item.dp.DonGiaKh = priceTableCus.DonGia;
                            item.dp.DonGiaNcc = priceTableSup.DonGia;
                            item.dp.MaRomooc = request.Romooc;
                            item.dp.ContNo = request.CONTNO;
                            item.dp.SealNp = request.SEALNP;
                            item.dp.SealHq = request.SEALHQ;
                            item.dp.DiemLayTraRong = request.DiemLayTraRong;

                            if (!string.IsNullOrEmpty(request.XeVanChuyen) && !string.IsNullOrEmpty(request.TaiXe) && item.dp.TrangThai == 19)
                            {
                                item.dp.TrangThai = 27;
                            }
                        }
                    }
                }

                var arrDataRequest = request.arrTransports.Where(x => !data.Select(y => y.dp.MaVanDon).Contains(x.MaVanDon));
                var getListNewTransport = await _context.VanDon.Where(x => arrDataRequest.Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToListAsync();

                foreach (var item in getListNewTransport)
                {
                    foreach (var itemRequest in arrDataRequest)
                    {
                        if (itemRequest.MaVanDon == item.MaVanDon)
                        {
                            var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == itemRequest.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                            if (checkGoodsType == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                            }

                            var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == itemRequest.MaDVT).Select(x => x.TenDvt).FirstOrDefaultAsync();
                            if (checkDVT == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                            }

                            var priceTableCus = await GetPriceTable(item.MaKh, item.MaCungDuong, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);
                            var priceTableSup = await GetPriceTable(request.DonViVanTai, item.MaCungDuong, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);

                            if (priceTableSup == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + item.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.MaPtvc
                                };
                            }

                            if (priceTableCus == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Khách Hàng: "
                                   + await _context.KhachHang.Where(x => x.MaKh == item.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + item.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.MaPtvc
                                };
                            }

                            int trangthai = 19;
                            if (!string.IsNullOrEmpty(request.XeVanChuyen) && !string.IsNullOrEmpty(request.TaiXe) && item.TrangThai == 8)
                            {
                                trangthai = 27;
                            }

                            await _context.DieuPhoi.AddAsync(new DieuPhoi()
                            {
                                MaVanDon = item.MaVanDon,
                                MaChuyen = handlingId,
                                MaSoXe = request.XeVanChuyen,
                                MaTaiXe = request.TaiXe,
                                MaLoaiHangHoa = itemRequest.MaLoaiHangHoa,
                                MaLoaiPhuongTien = request.PTVanChuyen,
                                MaDvt = itemRequest.MaDVT,
                                DonViVanTai = request.DonViVanTai,
                                BangGiaKh = priceTableCus.ID,
                                BangGiaNcc = priceTableSup.ID,
                                DonGiaKh = priceTableCus.DonGia,
                                DonGiaNcc = priceTableSup.DonGia,
                                MaRomooc = request.Romooc,
                                ContNo = request.CONTNO,
                                SealNp = request.SEALNP,
                                SealHq = request.SEALHQ,
                                SoKien = item.TongSoKien,
                                KhoiLuong = item.TongKhoiLuong,
                                TheTich = item.TongTheTich,
                                GhiChu = item.GhiChu,
                                DiemLayTraRong = request.DiemLayTraRong,
                                TrangThai = trangthai,
                                CreatedTime = DateTime.Now,
                            });
                        }
                    }
                }

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult() { isSuccess = true, Message = "Cập nhật điều phối thành công!" };
                }
                else
                {
                    return new BoolActionResult() { isSuccess = false, Message = "Cập nhật điều phối thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult() { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<UpdateTransportLess> GetTransportLessById(string transportId)
        {
            try
            {
                var transport = await _context.VanDon.Where(x => x.MaVanDon == transportId).FirstOrDefaultAsync();

                if (transport == null)
                {
                    return null;
                }

                return new UpdateTransportLess()
                {
                    MaKH = transport.MaKh,
                    MaVanDonKH = transport.MaVanDonKh,
                    LoaiVanDon = transport.LoaiVanDon,
                    HangTau = transport.HangTau,
                    TenTau = transport.Tau,
                    MaCungDuong = transport.MaCungDuong,
                    DiemLayHang = _context.CungDuong.Where(x => x.MaCungDuong == transport.MaCungDuong).Select(x => x.DiemDau).FirstOrDefault(),
                    DiemTraHang = _context.CungDuong.Where(x => x.MaCungDuong == transport.MaCungDuong).Select(x => x.DiemCuoi).FirstOrDefault(),
                    TongKhoiLuong = transport.TongKhoiLuong,
                    TongTheTich = transport.TongTheTich,
                    TongSoKien = transport.TongSoKien,
                    GhiChu = transport.GhiChu,
                    MaPTVC = transport.MaPtvc,
                    ThoiGianLayHang = transport.ThoiGianLayHang,
                    ThoiGianTraHang = transport.ThoiGianTraHang,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GetTransport> GetTransportById(string transportId)
        {
            try
            {
                var getTransport = from transport in _context.VanDon
                                   join road in _context.CungDuong
                                   on transport.MaCungDuong equals road.MaCungDuong
                                   where transport.MaVanDon == transportId
                                   select new { transport, road };

                return await getTransport.Select(x => new GetTransport()
                {
                    MaPTVC = x.transport.MaPtvc,
                    MaVanDonKH = x.transport.MaVanDonKh,
                    DiemDau = x.road.DiemDau,
                    DiemCuoi = x.road.DiemCuoi,
                    MaVanDon = x.transport.MaVanDon,
                    LoaiVanDon = x.transport.LoaiVanDon,
                    MaKh = x.transport.MaKh,
                    ThoiGianLayTraRong = x.transport.ThoiGianLayTraRong,
                    CungDuong = x.road.MaCungDuong,
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    TongKhoiLuong = x.transport.TongKhoiLuong,
                    TongTheTich = x.transport.TongTheTich,
                    TongSoKien = x.transport.TongSoKien,
                    ThoiGianCoMat = x.transport.ThoiGianCoMat,
                    ThoiGianHaCang = x.transport.ThoiGianHaCang,
                    ThoiGianHanLenh = x.transport.ThoiGianHanLenh,
                    ThoiGianLayHang = x.transport.ThoiGianLayHang,
                    ThoiGianTraHang = x.transport.ThoiGianTraHang,
                    ThoiGianTaoDon = x.transport.ThoiGianTaoDon,
                    HangTau = x.transport.HangTau,
                    TenTau = x.transport.Tau,
                    GhiChu = x.transport.GhiChu,
                    arrHandlings = _context.DieuPhoi.Where(y => y.MaVanDon == transportId).Select(y => new arrHandling()
                    {
                        DonViVanTai = y.DonViVanTai,
                        PTVanChuyen = y.MaLoaiPhuongTien,
                        LoaiHangHoa = y.MaLoaiHangHoa,
                        DonViTinh = y.MaDvt,
                        DiemLayTraRong = y.DiemLayTraRong,
                        KhoiLuong = y.KhoiLuong,
                        TheTich = y.TheTich,
                        SoKien = y.SoKien,
                    }).ToList(),
                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BoolActionResult> UpdateTransport(string transPortId, UpdateTransport request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (string.IsNullOrEmpty(request.MaVanDonKH))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng không bỏ trống Mã Vận Đơn Khách Hàng" };
                }

                if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() > 0)
                {
                    //if (request.ThoiGianLayTraRong == null)
                    //{
                    //    return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian lấy/trả rỗng" };
                    //}

                    if (request.LoaiVanDon == "nhap")
                    {
                        if (request.ThoiGianHanLenh == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                        }

                        if (request.ThoiGianLayTraRong != null && request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                        {
                            if (request.ThoiGianLayTraRong <= request.ThoiGianLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }

                            if (request.ThoiGianLayTraRong <= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }
                    }
                    else
                    {
                        if (request.ThoiGianHaCang == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                        }

                        if (request.ThoiGianLayTraRong != null && request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                        {
                            if (request.ThoiGianLayTraRong >= request.ThoiGianLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }
                            if (request.ThoiGianLayTraRong >= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }
                    }
                }

                if (request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                {
                    if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy hàng không được lơn hơn hoặc bằng Thời Gian Trả Hàng" };
                    }
                }

                var checkTransport = await _context.VanDon.Where(x => x.MaVanDon == transPortId).FirstOrDefaultAsync();

                if (checkTransport == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn không tồn tại" };
                }

                var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();
                if (checkRoad == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cung đường không tồn tại" };
                }

                if (checkTransport.TrangThai != 8)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vận đơn này không thể sửa nữa" };
                }

                if (request.LoaiVanDon == "nhap")
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() == 0)
                    {
                        request.ThoiGianLayTraRong = null;
                        request.ThoiGianCoMat = null;
                        request.ThoiGianHanLenh = null;
                    }
                    request.HangTau = null;
                    request.TenTau = null;
                }
                else
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() == 0)
                    {
                        request.ThoiGianHaCang = null;
                        request.ThoiGianLayTraRong = null;
                    }
                }

                checkTransport.MaPtvc = request.MaPTVC;
                checkTransport.HangTau = request.HangTau;
                checkTransport.Tau = request.TenTau;
                checkTransport.MaVanDonKh = request.MaVanDonKH;
                checkTransport.MaCungDuong = request.MaCungDuong;
                checkTransport.MaKh = request.MaKH;
                checkTransport.TongKhoiLuong = request.TongKhoiLuong;
                checkTransport.TongTheTich = request.TongTheTich;
                checkTransport.TongSoKien = request.TongSoKien;
                checkTransport.ThoiGianLayHang = request.ThoiGianLayHang;
                checkTransport.ThoiGianTraHang = request.ThoiGianTraHang;
                checkTransport.ThoiGianLayTraRong = request.ThoiGianLayTraRong;
                checkTransport.ThoiGianCoMat = request.ThoiGianCoMat;
                checkTransport.ThoiGianHaCang = request.ThoiGianHaCang;
                checkTransport.ThoiGianHanLenh = request.ThoiGianHanLenh;
                checkTransport.UpdatedTime = DateTime.Now;
                checkTransport.GhiChu = request.GhiChu;
                _context.VanDon.Update(checkTransport);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    _context.DieuPhoi.RemoveRange(_context.DieuPhoi.Where(x => x.MaVanDon == transPortId).ToList());

                    foreach (var item in request.arrHandlings)
                    {
                        var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == item.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();

                        if (checkVehicleType == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                        }

                        var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == item.LoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                        if (checkGoodsType == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                        }
                        var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == item.DonViTinh).Select(x => x.TenDvt).FirstOrDefaultAsync();
                        if (checkDVT == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                        }
                        var checkPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == request.MaPTVC).Select(x => x.TenPtvc).FirstOrDefaultAsync();
                        if (checkPTVC == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Phương Thức Vận Chuyển Không Tồn Tại" };
                        }

                        if (item.PTVanChuyen.Contains("CONT"))
                        {
                            var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemLayTraRong.Value).FirstOrDefaultAsync();
                            if (checkPlaceGetEmpty == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                            }
                        }

                        var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                        if (checkSupplier == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                        }

                        var checkPriceTable = from bg in _context.BangGia
                                              join hd in _context.HopDongVaPhuLuc
                                              on bg.MaHopDong equals hd.MaHopDong
                                              where (hd.MaKh == request.MaKH || hd.MaKh == item.DonViVanTai)
                                              && bg.MaCungDuong == request.MaCungDuong
                                              && bg.TrangThai == 4
                                              && bg.NgayApDung.Date <= DateTime.Now.Date
                                              && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                                              && bg.MaDvt == item.DonViTinh
                                              && bg.MaLoaiHangHoa == item.LoaiHangHoa
                                              && bg.MaLoaiPhuongTien == item.PTVanChuyen
                                              && bg.MaPtvc == request.MaPTVC
                                              select bg;

                        if (checkPriceTable.Count() == 2)
                        {
                            var priceTableSupplier = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").FirstOrDefaultAsync();
                            if (priceTableSupplier == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + checkPTVC
                                };
                            }

                            var priceTableCustomer = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").FirstOrDefaultAsync();
                            if (priceTableCustomer == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Khách Hàng: "
                                   + await _context.KhachHang.Where(x => x.MaKh == request.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + checkPTVC
                                };
                            }

                            var itemHandling = new DieuPhoi();
                            itemHandling.MaChuyen = request.MaPTVC.Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            itemHandling.MaVanDon = transPortId;
                            itemHandling.MaLoaiHangHoa = item.LoaiHangHoa;
                            itemHandling.MaLoaiPhuongTien = item.PTVanChuyen;
                            itemHandling.MaDvt = item.DonViTinh;
                            itemHandling.DonViVanTai = item.DonViVanTai;
                            itemHandling.BangGiaKh = priceTableCustomer.Id;
                            itemHandling.BangGiaNcc = priceTableSupplier.Id;
                            itemHandling.DonGiaKh = priceTableCustomer.DonGia;
                            itemHandling.DonGiaNcc = priceTableSupplier.DonGia;
                            itemHandling.KhoiLuong = item.KhoiLuong;
                            itemHandling.TheTich = item.TheTich;
                            itemHandling.SoKien = item.SoKien;
                            itemHandling.DiemLayTraRong = item.DiemLayTraRong;
                            itemHandling.TrangThai = 19;
                            itemHandling.CreatedTime = DateTime.Now;
                            await _context.DieuPhoi.AddAsync(itemHandling);
                        }
                        else
                        {
                            var priceTableSupplier = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").FirstOrDefaultAsync();
                            if (priceTableSupplier == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + checkPTVC
                                };
                            }

                            var priceTableCustomer = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").FirstOrDefaultAsync();
                            if (priceTableCustomer == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Khách Hàng: "
                                   + await _context.KhachHang.Where(x => x.MaKh == request.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + checkPTVC
                                };
                            }

                            return new BoolActionResult { isSuccess = false, Message = "Không có bảng giá khách hàng lẫn nhà cung cấp" };
                        }
                    }

                    var resultDP = await _context.SaveChangesAsync();

                    if (resultDP > 0)
                    {
                        await transaction.CommitAsync();
                        await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " Update Transport with Data: " + JsonSerializer.Serialize(request));
                        return new BoolActionResult { isSuccess = true, Message = "Cập nhật vận đơn thành công" };
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return new BoolActionResult { isSuccess = false, Message = "Cập nhật vận đơn thất bại" };
                    }
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + " Update Transport with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UpdateTransportLess(string transportId, UpdateTransportLess request)
        {
            try
            {
                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (request.ThoiGianLayHang != null || request.ThoiGianLayHang != null)
                {
                    if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Mốc thời gian không đúng" };
                    }
                }

                //if (request.TongKhoiLuong < 1 || request.TongTheTich < 1 || request.TongSoKien < 1)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Tổng khối lượng, tổng thể tích, tổng thùng hàng không được nhỏ hơn 1" };
                //}

                var checkTransport = await _context.VanDon.Where(x => x.MaVanDon == transportId).FirstOrDefaultAsync();

                if (checkTransport == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn không tồn tại" };
                }

                var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();
                if (checkRoad == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cung đường không tồn tại" };
                }

                if (checkTransport.TrangThai != 8)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vận đơn này không thể sửa nữa" };
                }

                checkTransport.MaVanDonKh = request.MaVanDonKH;
                checkTransport.MaKh = request.MaKH;
                checkTransport.MaCungDuong = request.MaCungDuong;
                checkTransport.LoaiVanDon = request.LoaiVanDon;
                checkTransport.HangTau = request.HangTau;
                checkTransport.Tau = request.TenTau;
                checkTransport.TongKhoiLuong = request.TongKhoiLuong;
                checkTransport.TongTheTich = request.TongTheTich;
                checkTransport.TongSoKien = request.TongSoKien;
                checkTransport.GhiChu = request.GhiChu;
                checkTransport.MaPtvc = request.MaPTVC;
                checkTransport.ThoiGianLayHang = request.ThoiGianLayHang;
                checkTransport.ThoiGianTraHang = request.ThoiGianTraHang;
                _context.VanDon.Update(checkTransport);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<PagedResponseCustom<ListTransport>> GetListTransport(string[] listCustomer, PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var listData = from transport in _context.VanDon
                           join
                           status in _context.StatusText
                           on
                           transport.TrangThai equals status.StatusId
                           join road in _context.CungDuong
                           on
                           transport.MaCungDuong equals road.MaCungDuong
                           join kh in _context.KhachHang
                           on
                           transport.MaKh equals kh.MaKh
                           where status.LangId == tempData.LangID
                           select new { transport, status, road, kh };

            if (listCustomer.Length > 0)
            {
                listData = listData.Where(x => listCustomer.Contains(x.kh.MaKh));
            }

            if (!string.IsNullOrEmpty(filter.maptvc))
            {
                listData = listData.Where(x => x.transport.MaPtvc == filter.maptvc);
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.transport.MaVanDon.Contains(filter.Keyword) || x.transport.MaVanDonKh.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.customerId))
            {
                listData = listData.Where(x => x.transport.MaKh == filter.customerId);
            }

            if (!string.IsNullOrEmpty(filter.statusId))
            {
                listData = listData.Where(x => x.status.StatusId == int.Parse(filter.statusId));
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                listData = listData.Where(x => x.transport.ThoiGianTaoDon.Date >= filter.fromDate.Value.Date && x.transport.ThoiGianTaoDon.Date <= filter.toDate.Value.Date);
            }

            var totalCount = await listData.CountAsync();

            var pagedData = await listData.OrderByDescending(x => x.transport.MaVanDon).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListTransport()
            {
                MaVanDonKH = x.transport.MaVanDonKh,
                MaPTVC = x.transport.MaPtvc,
                MaVanDon = x.transport.MaVanDon,
                MaKH = x.transport.MaKh,
                TenKH = x.kh.TenKh,
                LoaiVanDon = x.transport.LoaiVanDon,
                MaCungDuong = x.road.MaCungDuong,
                TenCungDuong = x.road.TenCungDuong,
                DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                TongSoKien = x.transport.TongSoKien,
                TongKhoiLuong = x.transport.TongKhoiLuong,
                TongTheTich = x.transport.TongTheTich,
                ThoiGianLayHang = x.transport.ThoiGianLayHang,
                ThoiGianTaoDon = x.transport.ThoiGianTaoDon,
                ThoiGianTraHang = x.transport.ThoiGianTraHang,
                ThoiGianCoMat = x.transport.ThoiGianCoMat,
                ThoiGianLayTraRong = x.transport.ThoiGianLayTraRong,
                ThoiGianHanLenh = x.transport.ThoiGianHanLenh,
                ThoiGianHaCang = x.transport.ThoiGianHaCang,
                HangTau = x.transport.HangTau,
                TongThungHang = x.transport.TongThungHang,
                MaTrangThai = x.status.StatusId,
                TrangThai = x.status.StatusContent,
            }).ToListAsync();

            return new PagedResponseCustom<ListTransport>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<PagedResponseCustom<ListHandling>> GetListHandling(string transportId, string[] customers, PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var listData = from vd in _context.VanDon
                           join
                           dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon
                           join tt in _context.StatusText
                           on dp.TrangThai equals tt.StatusId
                           where tt.LangId == tempData.LangID && vd.MaPtvc != "LTL" && vd.MaPtvc != "LCL"
                           select new { vd, dp, tt };

            if (!string.IsNullOrEmpty(transportId))
            {
                listData = listData.Where(x => x.vd.MaVanDon == transportId);
            }

            if (customers.Length > 0)
            {
                listData = listData.Where(x => customers.Contains(x.vd.MaKh));
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.dp.MaSoXe.Contains(filter.Keyword) || x.dp.ContNo.Contains(filter.Keyword) || x.vd.MaVanDonKh.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.statusId))
            {
                listData = listData.Where(x => x.dp.TrangThai == int.Parse(filter.statusId));
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                listData = listData.Where(x => x.dp.CreatedTime.Date >= filter.fromDate.Value.Date && x.dp.CreatedTime.Date <= filter.toDate.Value.Date);
            }

            var totalCount = await listData.CountAsync();

            var getPoint = from point in _context.DiaDiem select point;

            var pagedData = await listData.OrderByDescending(x => x.vd.MaVanDon).ThenBy(x => x.dp.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListHandling()
            {
                DiemLayHang = getPoint.Where(y => y.MaDiaDiem == _context.CungDuong.Where(z => z.MaCungDuong == x.vd.MaCungDuong).Select(x => x.DiemDau).FirstOrDefault()).Select(x => x.TenDiaDiem).FirstOrDefault(),
                DiemTraHang = getPoint.Where(y => y.MaDiaDiem == _context.CungDuong.Where(z => z.MaCungDuong == x.vd.MaCungDuong).Select(x => x.DiemCuoi).FirstOrDefault()).Select(x => x.TenDiaDiem).FirstOrDefault(),
                HangTau = x.vd.HangTau,
                MaVanDonKH = x.vd.MaVanDonKh,
                MaKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
                MaPTVC = x.vd.MaPtvc,
                CungDuong = _context.CungDuong.Where(y => y.MaCungDuong == x.vd.MaCungDuong).Select(x => x.TenCungDuong).FirstOrDefault(),
                MaVanDon = x.dp.MaVanDon,
                PhanLoaiVanDon = x.vd.LoaiVanDon,
                MaDieuPhoi = x.dp.Id,
                DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                MaSoXe = x.dp.MaSoXe,
                PTVanChuyen = x.dp.MaLoaiPhuongTien,
                MaRomooc = x.dp.MaRomooc,
                ContNo = x.dp.ContNo,
                KhoiLuong = x.dp.KhoiLuong,
                SoKien = x.dp.SoKien,
                TheTich = x.dp.TheTich,
                TrangThai = x.tt.StatusContent,
                statusId = x.tt.StatusId,
                ThoiGianTaoDon = x.vd.ThoiGianTaoDon
            }).ToListAsync();

            return new PagedResponseCustom<ListHandling>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<PagedResponseCustom<ListHandling>> GetListHandlingLess(string[] customers, PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var listData = from vd in _context.VanDon
                               join dp in _context.DieuPhoi
                               on vd.MaVanDon equals dp.MaVanDon into vddpp
                               from vddp in vddpp.DefaultIfEmpty()
                               join tt in _context.StatusText
                               on vddp.TrangThai equals tt.StatusId into ttdpp
                               from ttdp in ttdpp.DefaultIfEmpty()
                               where (ttdp.LangId == tempData.LangID || ttdp.LangId == null) && (vd.MaPtvc == "LTL" || vd.MaPtvc == "LCL")
                               select new { vd, vddp, ttdp };

                var da = listData.ToQueryString();

                if (customers.Length > 0)
                {
                    listData = listData.Where(x => customers.Contains(x.vd.MaKh));
                }

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.vddp.MaSoXe.Contains(filter.Keyword) || x.vddp.ContNo.Contains(filter.Keyword) || x.vd.MaVanDonKh.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.statusId))
                {
                    if (filter.statusId == "null")
                    {
                        listData = listData.Where(x => x.vddp.MaChuyen == null);
                    }
                    else
                    {
                        listData = listData.Where(x => x.vddp.TrangThai == int.Parse(filter.statusId));
                    }
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    listData = listData.Where(x => x.vddp.CreatedTime.Date >= filter.fromDate.Value.Date && x.vddp.CreatedTime.Date <= filter.toDate.Value.Date);
                }

                var totalCount = await listData.CountAsync();

                var getPoint = from point in _context.DiaDiem select point;

                var pagedData = await listData.OrderByDescending(x => x.vd.CreatedTime).ThenByDescending(x => x.vddp.MaChuyen).ThenBy(x => x.vddp.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListHandling()
                {
                    MaChuyen = x.vddp.MaChuyen,
                    DiemLayHang = getPoint.Where(y => y.MaDiaDiem == _context.CungDuong.Where(z => z.MaCungDuong == x.vd.MaCungDuong).Select(x => x.DiemDau).FirstOrDefault()).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = getPoint.Where(y => y.MaDiaDiem == _context.CungDuong.Where(z => z.MaCungDuong == x.vd.MaCungDuong).Select(x => x.DiemCuoi).FirstOrDefault()).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    HangTau = x.vd.HangTau,
                    MaVanDonKH = x.vd.MaVanDonKh,
                    MaKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                    DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.vddp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
                    MaPTVC = x.vd.MaPtvc,
                    CungDuong = _context.CungDuong.Where(y => y.MaCungDuong == x.vd.MaCungDuong).Select(x => x.TenCungDuong).FirstOrDefault(),
                    MaVanDon = x.vd.MaVanDon,
                    PhanLoaiVanDon = x.vd.LoaiVanDon,
                    MaDieuPhoi = x.vddp.Id,
                    DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vddp.DiemLayTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    MaSoXe = x.vddp.MaSoXe,
                    PTVanChuyen = x.vddp.MaLoaiPhuongTien,
                    MaRomooc = x.vddp.MaRomooc,
                    ContNo = x.vddp.ContNo,
                    KhoiLuong = x.vddp.KhoiLuong,
                    SoKien = x.vddp.SoKien,
                    TheTich = x.vddp.TheTich,
                    TrangThai = x.ttdp.StatusContent,
                    statusId = x.ttdp.StatusId,
                    ThoiGianTaoDon = x.vd.ThoiGianTaoDon
                }).ToListAsync();

                return new PagedResponseCustom<ListHandling>()
                {
                    dataResponse = pagedData,
                    totalCount = totalCount,
                    paginationFilter = validFilter
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GetHandling> GetHandlingById(int id)
        {
            try
            {
                var getHandling = from vd in _context.VanDon
                                  join
                                  dp in _context.DieuPhoi
                                  on vd.MaVanDon equals dp.MaVanDon
                                  where dp.Id == id
                                  select new { vd, dp };

                var data = await getHandling.FirstOrDefaultAsync();

                var getRoad = await _context.CungDuong.Where(x => x.MaCungDuong == data.vd.MaCungDuong).FirstOrDefaultAsync();

                var RoadDetail = new RoadDetail()
                {
                    MaCungDuong = getRoad.MaCungDuong,
                    TenCungDuong = getRoad.TenCungDuong,
                    DiemLayHang = await _context.DiaDiem.Where(x => x.MaDiaDiem == getRoad.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefaultAsync(),
                    DiemTraHang = await _context.DiaDiem.Where(x => x.MaDiaDiem == getRoad.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefaultAsync(),
                    DiemLayRong = await _context.DiaDiem.Where(x => x.MaDiaDiem == data.dp.DiemLayTraRong).Select(x => x.TenDiaDiem).FirstOrDefaultAsync()
                };

                return new GetHandling()
                {
                    CungDuong = RoadDetail,
                    PhanLoaiVanDon = data.vd.LoaiVanDon,
                    MaLoaiHangHoa = data.dp.MaLoaiHangHoa,
                    MaDVT = data.dp.MaDvt,
                    MaKh = data.vd.MaKh,
                    MaVanDon = data.vd.MaVanDon,
                    MaCungDuong = data.vd.MaCungDuong,
                    MaSoXe = data.dp.MaSoXe,
                    MaTaiXe = data.dp.MaTaiXe,
                    DonViVanTai = data.dp.DonViVanTai,
                    PTVanChuyen = data.dp.MaLoaiPhuongTien,
                    TenTau = data.vd.Tau,
                    HangTau = data.vd.HangTau,
                    MaRomooc = data.dp.MaRomooc,
                    DiemLayRong = data.dp.DiemLayTraRong,
                    ContNo = data.dp.ContNo,
                    SealNp = data.dp.SealNp,
                    SealHq = data.dp.SealHq,
                    TongKhoiLuong = data.vd.TongKhoiLuong,
                    TongTheTich = data.vd.TongTheTich,
                    TongSoKien = data.vd.TongSoKien,
                    KhoiLuong = data.dp.KhoiLuong,
                    TheTich = data.dp.TheTich,
                    SoKien = data.dp.SoKien,
                    GhiChu = data.dp.GhiChu,
                    GhiChuVanDon = data.vd.GhiChu,
                    ThoiGianLayTraRong = data.vd.ThoiGianLayTraRong,
                    ThoiGianHaCang = data.vd.ThoiGianHaCang,
                    ThoiGianHanLenh = data.vd.ThoiGianHanLenh,
                    ThoiGianCoMat = data.vd.ThoiGianCoMat,
                    ThoiGianLayHang = data.vd.ThoiGianLayHang,
                    ThoiGianTraHang = data.vd.ThoiGianTraHang,

                    ThoiGianLayTraRongThucTe = data.dp.ThoiGianLayTraRongThucTe,
                    ThoiGianCoMatThucTe = data.dp.ThoiGianCoMatThucTe,
                    ThoiGianLayHangThucTe = data.dp.ThoiGianLayHangThucTe,
                    ThoiGianTraHangThucTe = data.dp.ThoiGianTraHangThucTe,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BoolActionResult> SetRunning(int id)
        {
            try
            {
                var handling = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (handling == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã điều phối không tồn tại" };
                }

                var getTransport = await _context.VanDon.Where(x => x.MaVanDon == handling.MaVanDon).FirstOrDefaultAsync();

                if (handling.TrangThai == 18)
                {
                    //if (handling.ThoiGianTraHangThucTe == null)
                    //{
                    //    return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Trả Hàng thực tế " };
                    //}
                    //if (handling.ThoiGianLayHangThucTe == null)
                    //{
                    //    return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Lấy Hàng thực tế " };
                    //}

                    //if (handling.MaLoaiPhuongTien.Contains("CONT"))
                    //{
                    //    if (handling.ThoiGianLayTraRongThucTe == null)
                    //    {
                    //        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Lấy/Trả rỗng thực tế " };
                    //    }
                    //}

                    handling.TrangThai = 20;
                    handling.ThoiGianHoanThanh = DateTime.Now;
                    _context.Update(handling);

                    var result1 = await _context.SaveChangesAsync();
                    if (result1 > 0)
                    {
                        var checkAllHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == handling.MaVanDon).ToListAsync();
                        if (checkAllHandling.Where(x => x.TrangThai == 20 || x.TrangThai == 21).Count() == checkAllHandling.Count())
                        {
                            getTransport.TrangThai = 22;
                            getTransport.ThoiGianHoanThanh = DateTime.Now;
                            _context.Update(getTransport);

                            var result = await _context.SaveChangesAsync();
                            if (result > 0)
                            {
                                await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set " + handling.MaVanDon + " completed");
                                return new BoolActionResult { isSuccess = true, Message = "Mã vận đơn " + handling.MaVanDon + " đã hoàn thành" };
                            }
                            else
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                            }
                        }

                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set " + handling.Id + " completed");
                        return new BoolActionResult { isSuccess = true, Message = "Tài Xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " và Xe: " + handling.MaSoXe + " đã hoàn thành chuyến" };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }

                if (handling.MaLoaiPhuongTien.Contains("CONT") && handling.TrangThai == 17)
                {
                    if (string.IsNullOrEmpty(handling.ContNo))
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật ContNo trước đã" };
                    }

                    handling.TrangThai = 18;
                    _context.Update(handling);

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handling.Id + "Shipping");
                        return new BoolActionResult { isSuccess = true, Message = "Đã điều phối xe: " + handling.MaSoXe + " và tài xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " vận chuyển hàng" };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }

                if (handling.TrangThai == 27)
                {
                    string status = "";
                    if (handling.MaLoaiPhuongTien.Contains("TRUCK"))
                    {
                        handling.TrangThai = 18;
                        status = " vận chuyển hàng";
                    }
                    if (handling.MaLoaiPhuongTien.Contains("CONT"))
                    {
                        handling.TrangThai = 17;
                        status = " đi lấy rỗng";
                    }
                    getTransport.TrangThai = 10;

                    _context.Update(handling);
                    _context.Update(getTransport);

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handling.Id + status);
                        return new BoolActionResult { isSuccess = true, Message = "Đã điều phối xe: " + handling.MaSoXe + " và tài xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + status };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }

                return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi " };
            }
            catch (Exception ex)
            {
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + "  SetRuning with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> SetRunningLess(string handlingId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == handlingId).ToListAsync();
                var getlistTransports = await _context.VanDon.Where(x => getListHandling.Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToListAsync();

                if (getlistTransports.Select(x => x.MaPtvc).FirstOrDefault() == "LCL" && getListHandling.Select(x => x.TrangThai).FirstOrDefault() == 17)
                {
                    if (string.IsNullOrEmpty(getListHandling.Select(x => x.ContNo).FirstOrDefault()))
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật ContNo trước đã" };
                    }

                    getListHandling.ForEach(x =>
                    {
                        x.TrangThai = 18;
                    });

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handlingId + "Shipping");
                        await transaction.CommitAsync();
                        return new BoolActionResult { isSuccess = true, Message = "Đã điều phối xe vận chuyển hàng" };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }

                if (getListHandling.Select(x => x.TrangThai).FirstOrDefault() == 27)
                {
                    string status = "";
                    if (getlistTransports.Select(x => x.MaPtvc).FirstOrDefault() == "LTL")
                    {
                        getListHandling.ForEach(x =>
                        {
                            x.TrangThai = 18;
                        });
                        status = " vận chuyển hàng";
                    }

                    if (getlistTransports.Select(x => x.MaPtvc).FirstOrDefault() == "LCL")
                    {
                        getListHandling.ForEach(x =>
                        {
                            x.TrangThai = 17;
                        });
                        status = " đi lấy rỗng";
                    }

                    getlistTransports.ForEach(x =>
                    {
                        x.TrangThai = 10;
                    });

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handlingId + status);
                        await transaction.CommitAsync();
                        return new BoolActionResult { isSuccess = true, Message = "Đã điều phối xe " + status };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }
                return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi " };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CancelHandling(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var getByid = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (getByid == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Lệnh điều phối không tồn tại" };
                }

                if (getByid.TrangThai != 19 || getByid.TrangThai != 27)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể hủy lệnh điều phối này" };
                }

                getByid.TrangThai = 21;
                _context.Update(getByid);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var getAllHandlingOfTransport = await _context.DieuPhoi.Where(x => x.MaVanDon == getByid.MaVanDon).ToListAsync();

                    if (getAllHandlingOfTransport.Where(x => x.TrangThai == 21).Count() == getAllHandlingOfTransport.Count())
                    {
                        var getTransport = await _context.VanDon.Where(x => x.MaVanDon == getByid.MaVanDon).FirstOrDefaultAsync();

                        getTransport.TrangThai = 11;
                        _context.Update(getTransport);

                        var result1 = await _context.SaveChangesAsync();
                        if (result1 > 0)
                        {
                            return new BoolActionResult { isSuccess = true, Message = "Đã hủy lệnh điều phối và hủy vận đơn thành công" };
                        }
                        else
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Hủy lệnh thất bại" };
                        }
                    }

                    if (getAllHandlingOfTransport.Where(x => x.TrangThai == 20 || x.TrangThai == 21).Count() == getAllHandlingOfTransport.Count())
                    {
                        var getTransport = await _context.VanDon.Where(x => x.MaVanDon == getByid.MaVanDon).FirstOrDefaultAsync();
                        getTransport.TrangThai = 22;
                        getTransport.ThoiGianHoanThanh = DateTime.Now;
                        _context.Update(getTransport);

                        var result3 = await _context.SaveChangesAsync();
                        if (result > 0)
                        {
                            await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set TransportId: " + getTransport.MaVanDon + " completed");
                            return new BoolActionResult { isSuccess = true, Message = "Mã vận đơn " + getTransport.MaVanDon + " đã hoàn thành" };
                        }
                        else
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                        }
                    }

                    await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " set  Cancel handling " + id);
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Đã hủy lệnh điều phối thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hủy lệnh thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + "  CancelHandling with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CancelHandlingLess(string handlingId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var listHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == handlingId).ToListAsync();
                var listHandlingCancel = new List<long>();

                foreach (var item in listHandling)
                {
                    if (item.TrangThai == 19 || item.TrangThai == 27)
                    {
                        listHandlingCancel.Add(item.Id);
                    }
                }

                if (listHandlingCancel.Count == 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể hủy lệnh điều phối này" };
                }

                listHandling = listHandling.Where(x => listHandlingCancel.Contains(x.Id)).ToList();

                listHandling.ForEach(x =>
                {
                    x.TrangThai = 21;
                    x.UpdatedTime = DateTime.Now;
                });

                var listTransports = await _context.VanDon.Where(x => listHandling.Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToListAsync();
                listTransports.ForEach(x =>
                {
                    x.TrangThai = 11;
                    x.UpdatedTime = DateTime.Now;
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Đã hủy lệnh điều phối và hủy vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hủy lệnh điều phối và hủy vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult() { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UpdateHandling(int id, UpdateHandling request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var checkById = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (checkById == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã điều phối không tồn tại" };
                }

                if (checkById.TrangThai == 20)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể cập nhật chuyến này nữa" };
                }

                var getTransport = await _context.VanDon.Where(x => x.MaVanDon == checkById.MaVanDon).FirstOrDefaultAsync();

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();

                if (checkVehicleType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                }

                var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == request.LoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                if (checkGoodsType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                }
                var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == request.DonViTinh).Select(x => x.TenDvt).FirstOrDefaultAsync();
                if (checkDVT == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                }
                //var checkPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == request.).Select(x => x.TenPtvc).FirstOrDefaultAsync();
                //if (checkPTVC == null)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Phương Thức Vận Chuyển Không Tồn Tại" };
                //}

                var checkDriver = await _context.TaiXe.Where(x => x.MaTaiXe == request.MaTaiXe).FirstOrDefaultAsync();
                if (checkDriver == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = " Tài xế không tồn tại \r\n" };
                }

                var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == request.MaSoXe).FirstOrDefaultAsync();
                if (checkVehicle == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = " Xe vận chuyển không tồn tại \r\n" };
                }

                if (!request.PTVanChuyen.Contains(checkVehicle.MaLoaiPhuongTien))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại xe không khớp với xe vận chuyển \r\n" };
                }

                if (request.PTVanChuyen.Contains("CONT"))
                {
                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.DiemLayTraRong.Value).FirstOrDefaultAsync();
                    if (checkPlaceGetEmpty == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                    }
                }
                else
                {
                    request.DiemLayTraRong = null;
                    request.ThoiGianLayTraRongThucTe = null;
                    request.ThoiGianCoMatThucTe = null;
                }

                //if (getTransport.MaPtvc == "FCL" || getTransport.MaPtvc == "FTL")
                //{
                //    var checkVehicleOfTransport = await _context.DieuPhoi.Where(x => x.MaVanDon == getTransport.MaVanDon && x.MaSoXe == request.MaSoXe && x.Id != id).FirstOrDefaultAsync();

                //    if (checkVehicleOfTransport != null)
                //    {
                //        return new BoolActionResult { isSuccess = false, Message = "Xe này đã được điều phối trước đó, vui lòng chọn xe khác" };
                //    }
                //}

                if (checkById.TrangThai == 17 || checkById.TrangThai == 18 || checkById.TrangThai == 20)
                {
                    checkById.ThoiGianLayHangThucTe = request.ThoiGianLayHangThucTe;
                    checkById.ThoiGianTraHangThucTe = request.ThoiGianTraHangThucTe;
                    checkById.ThoiGianLayTraRongThucTe = request.ThoiGianLayTraRongThucTe;
                    checkById.ThoiGianCoMatThucTe = request.ThoiGianCoMatThucTe;
                }

                if (checkById.TrangThai == 19 || checkById.TrangThai == 27)
                {
                    var priceSup = await GetPriceTable(request.DonViVanTai, getTransport.MaCungDuong, request.DonViTinh, request.LoaiHangHoa, request.PTVanChuyen, getTransport.MaPtvc);
                    var priceCus = await GetPriceTable(getTransport.MaKh, getTransport.MaCungDuong, request.DonViTinh, request.LoaiHangHoa, request.PTVanChuyen, getTransport.MaPtvc);

                    if (priceSup == null)
                    {
                        return new BoolActionResult
                        {
                            isSuccess = false,
                            Message = "Đơn Vị Vận Tải: "
                                  + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                               + " chưa có bảng giá cho Cung Đường: " + getTransport.MaCungDuong +
                               ", Phương Tiện Vận Chuyển: " + request.PTVanChuyen +
                               ", Loại Hàng Hóa:" + request.LoaiHangHoa +
                               ", Đơn Vị Tính: " + request.DonViTinh +
                               ", Phương thức vận chuyển: " + getTransport.MaPtvc
                        };
                    }

                    if (priceCus == null)
                    {
                        return new BoolActionResult
                        {
                            isSuccess = false,
                            Message = "Khách Hàng: "
                                  + await _context.KhachHang.Where(x => x.MaKh == getTransport.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                               + " chưa có bảng giá cho Cung Đường: " + getTransport.MaCungDuong +
                               ", Phương Tiện Vận Chuyển: " + request.PTVanChuyen +
                               ", Loại Hàng Hóa:" + request.LoaiHangHoa +
                               ", Đơn Vị Tính: " + request.DonViTinh +
                               ", Phương thức vận chuyển: " + getTransport.MaPtvc
                        };
                    }

                    checkById.MaLoaiHangHoa = request.LoaiHangHoa;
                    checkById.MaLoaiPhuongTien = request.PTVanChuyen;
                    checkById.MaDvt = request.DonViTinh;
                    checkById.DonViVanTai = request.DonViVanTai;
                    checkById.BangGiaNcc = priceSup.ID;
                    checkById.DonGiaNcc = priceSup.DonGia;
                    checkById.BangGiaKh = priceCus.ID;
                    checkById.DonGiaKh = priceCus.DonGia;
                    checkById.SoKien = request.SoKien;
                    checkById.KhoiLuong = request.KhoiLuong;
                    checkById.TheTich = request.TheTich;

                    checkById.DiemLayTraRong = request.DiemLayTraRong;
                    checkById.TrangThai = 27;
                    getTransport.TrangThai = 9;
                }

                checkById.ContNo = request.ContNo;
                checkById.SealHq = request.SealHq;
                checkById.SealNp = request.SealNp;
                checkById.MaSoXe = request.MaSoXe;
                checkById.MaTaiXe = request.MaTaiXe;
                checkById.MaRomooc = request.MaRomooc;
                checkById.GhiChu = request.GhiChu;
                checkById.UpdatedTime = DateTime.Now;

                _context.Update(getTransport);
                _context.Update(checkById);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " update handling with Data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Điều Phối Chuyến Thành Công!" };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = "Điều Phối Chuyến Thất Bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CloneHandling(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkHangling = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (checkHangling.TrangThai != 19)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể copy dữ liệu điều phối này" };
                }

                await _context.DieuPhoi.AddAsync(new DieuPhoi()
                {
                    MaVanDon = checkHangling.MaVanDon,
                    TrangThai = checkHangling.TrangThai,
                    CreatedTime = DateTime.Now,
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Copy dữ liệu điều phối thành công!" };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = "Copy dữ liệu điều phối thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> RemoveHandling(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkHangling = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (checkHangling.TrangThai != 19)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể xóa điều phối này" };
                }

                var checkCount = await _context.DieuPhoi.Where(x => x.MaVanDon == checkHangling.MaVanDon).ToListAsync();
                if (checkCount.Count <= 1)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể xóa điều phối này" };
                }

                _context.DieuPhoi.Remove(checkHangling);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Xóa dữ liệu điều phối thành công!" };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = "Xóa dữ liệu điều phối thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<List<Attachment>> GetListImageByHandlingId(int handlingId)
        {
            var list = await _context.Attachment.Where(x => x.DieuPhoiId == handlingId).ToListAsync();
            return list.Select(x => new Attachment()
            {
                Id = x.Id,
                FileName = x.FileName,
                FileType = x.FileType,
                // FilePath =Path.Combine(_common.GetFile(x.FilePath)),
                UploadedTime = x.UploadedTime
            }).ToList();
        }

        public async Task<Attachment> GetImageById(int id)
        {
            var image = await _context.Attachment.Where(x => x.Id == id).FirstOrDefaultAsync();

            return new Attachment
            {
                FilePath = Path.Combine(_common.GetFile(image.FilePath)),
                FileName = image.FileName,
                FileType = image.FileType
            };
        }

        public async Task<BoolActionResult> DeleteImageById(int imageId)
        {
            try
            {
                var image = await _context.Attachment.Where(x => x.Id == imageId).FirstOrDefaultAsync();

                if (image == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hình ảnh không tồn tại" };
                }

                await _common.DeleteFileAsync(image.FileName, image.FilePath);

                _context.Attachment.Remove(image);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Xóa hình ảnh thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Xóa hình ảnh thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UploadFile(UploadImagesHandling request)
        {
            var PathFolder = $"Transport/{request.transportId}/{request.handlingId}";

            if (request.files.Files.Count < 1)
            {
                return new BoolActionResult { isSuccess = false, Message = "Không có file nào" };
            }

            var checkTrasnport = await _context.DieuPhoi.Where(x => x.MaVanDon == request.transportId && x.Id == request.handlingId).FirstOrDefaultAsync();

            if (checkTrasnport == null)
            {
                return new BoolActionResult { isSuccess = false, Message = "Điều phối không tồn tại trong vận đơn" };
            }

            foreach (var fileItem in request.files.Files)
            {
                var originalFileName = ContentDispositionHeaderValue.Parse(fileItem.ContentDisposition).FileName.Trim('"');
                var supportedTypes = new[] { "jpg", "jpeg", "png" };
                var fileExt = Path.GetExtension(originalFileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    return new BoolActionResult { isSuccess = false, Message = "File không được hỗ trợ" };
                }

                var reNameFile = originalFileName.Replace(originalFileName.Substring(0, originalFileName.LastIndexOf('.')), Guid.NewGuid().ToString());
                var fileName = $"{reNameFile.Substring(0, reNameFile.LastIndexOf('.'))}{Path.GetExtension(reNameFile)}";

                var attachment = new Attachment()
                {
                    FileName = fileName,
                    FilePath = _common.GetFileUrl(fileName, PathFolder),
                    FileSize = fileItem.Length,
                    FileType = Path.GetExtension(fileName),
                    FolderName = "Transport",
                    DieuPhoiId = request.handlingId,
                    UploadedTime = DateTime.Now
                };

                var add = await _common.AddAttachment(attachment);

                if (add.isSuccess == false)
                {
                    return new BoolActionResult { isSuccess = false, Message = add.Message };
                }
                await _common.SaveFileAsync(fileItem.OpenReadStream(), fileName, PathFolder);
            }

            await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " upload file with data: " + JsonSerializer.Serialize(request));
            return new BoolActionResult { isSuccess = true, Message = "Upload hình ảnh thành công" };
        }

        private async Task<GetPriceListRequest> GetPriceTable(string MaKH, string MaCungDuong, string MaDVT, string LoaiHangHoa, string LoaiPhuongTien, string MaPTVC)
        {
            var checkPriceTable = from bg in _context.BangGia
                                  join hd in _context.HopDongVaPhuLuc
                                  on bg.MaHopDong equals hd.MaHopDong
                                  where hd.MaKh == MaKH
                                  && bg.MaCungDuong == MaCungDuong
                                  && bg.TrangThai == 4
                                  && bg.NgayApDung.Date <= DateTime.Now.Date
                                  && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                                  && bg.MaDvt == MaDVT
                                  && bg.MaLoaiHangHoa == LoaiHangHoa
                                  && bg.MaLoaiPhuongTien == LoaiPhuongTien
                                  && bg.MaPtvc == MaPTVC
                                  select bg;

            if (checkPriceTable.Count() == 1)
            {
                return await checkPriceTable.Select(x => new GetPriceListRequest()
                {
                    ID = x.Id,
                    MaCungDuong = x.MaCungDuong,
                    DonGia = x.DonGia,
                    MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                    MaLoaiHangHoa = x.MaLoaiHangHoa,
                    MaLoaiDoiTac = x.MaLoaiDoiTac,
                    MaDVT = x.MaDvt,
                    MaPTVC = x.MaPtvc
                }).FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }
    }
}