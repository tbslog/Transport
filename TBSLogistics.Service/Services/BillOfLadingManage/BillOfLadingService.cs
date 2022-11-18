using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;
using TBSLogistics.Service.Repository.RoadManage;
using TBSLogistics.Service.Services.RomoocManage;

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

        public async Task<ListPoint> LoadDataRoadTransportByCusId(string customerId)
        {
            var checkCus = await _context.KhachHang.Where(x => x.MaKh == customerId && x.MaLoaiKh == "KH").FirstOrDefaultAsync();

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
                         MaCungDuong = g.Key.MaCungDuong,
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
                int truck = 0;
                foreach (var item in request.DieuPhoi)
                {
                    truck += 1;

                    if (string.IsNullOrEmpty(item.KhoiLuong.ToString()) || string.IsNullOrEmpty(item.TheTich.ToString()))
                    {
                        try
                        {
                            if (item.KhoiLuong < 1 || item.TheTich < 1)
                            {
                                ErrorValidate += "Thùng hàng số " + truck + " Không được để Khối Lượng hoặc Thể Tích không được nhỏ hơn 1";
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorValidate += "Thùng hàng số " + truck + " Khối Lượng hoặc Thể Tích không đúng định dạng";
                        }
                    }

                    if (string.IsNullOrEmpty(item.DonViVanTai))
                    {
                        ErrorValidate += "Thùng hàng số " + truck + " Không được để trống Đơn vị vận tải";
                    }

                    if (string.IsNullOrEmpty(item.MaTaiXe) || string.IsNullOrEmpty(item.MaSoXe))
                    {
                        ErrorValidate += "Thùng hàng số " + truck + " Vui lòng chọn tài xé và xe vận chuyển \r\n";
                    }

                    if (string.IsNullOrEmpty(item.MaDVT) || string.IsNullOrEmpty(item.MaLoaiHangHoa) || string.IsNullOrEmpty(item.MaLoaiPhuongTien) || string.IsNullOrEmpty(item.MaPTVC))
                    {
                        ErrorValidate += "Thùng hàng số " + truck + " Vui lòng không bỏ trống Loại Hàng Hóa hoặc Loại Phươn Tiện \r\n";
                    }

                    if (item.MaLoaiPhuongTien.Contains("CONT"))
                    {
                        if (string.IsNullOrEmpty(item.DiemLayTraRong.ToString()))
                        {
                            ErrorValidate += "Thùng hàng số " + truck + " Vui lòng chọn điểm trả rỗng \r\n";
                        }

                        if (item.ThoiGianLayTraRong == null)
                        {
                            ErrorValidate += "Thùng hàng số " + truck + " Vui lòng nhập thời trả rỗng \r\n";
                        }

                        if (transport.LoaiVanDon == "xuat")
                        {
                            if (string.IsNullOrEmpty(item.HangTau) || string.IsNullOrEmpty(item.TenTau))
                            {
                                ErrorValidate += "Thùng hàng số " + truck + " không được để trống hãng tàu và tên tàu \r\n";
                            }

                            if (string.IsNullOrEmpty(item.ThoiGianCatMang.ToString()))
                            {
                                ErrorValidate += "Thùng hàng số " + truck + " không được để trống thời gian cắt máng \r\n";
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(ErrorValidate))
                    {
                        if (!item.DonViVanTai.Contains("TBSL"))
                        {
                            var checkNcc = await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).FirstOrDefaultAsync();

                            if (checkNcc == null)
                            {
                                ErrorValidate += "Thùng hàng số " + truck + " Nhà cung cấp không tồn tại \r\n";
                            }
                        }

                        var checkDriver = await _context.TaiXe.Where(x => x.MaTaiXe == item.MaTaiXe).FirstOrDefaultAsync();
                        if (checkDriver == null)
                        {
                            ErrorValidate += "Thùng hàng số " + truck + " Tài xế không tồn tại \r\n";
                        }

                        var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == item.MaSoXe).FirstOrDefaultAsync();
                        if (checkVehicle == null)
                        {
                            ErrorValidate += "Thùng hàng số " + truck + " Xe vận chuyển không tồn tại \r\n";
                        }

                        if (checkVehicle.MaLoaiPhuongTien != item.MaLoaiPhuongTien)
                        {
                            ErrorValidate += "Thùng hàng số " + truck + " Loại xe không khớp với xe vận chuyển \r\n";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ErrorValidate))
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                using var transaction = _context.Database.BeginTransaction();

                transport.TrangThai = 9;
                _context.VanDon.Update(transport);

                int thung = 0;
                foreach (var item in request.DieuPhoi)
                {
                    thung += 1;

                    var checkPriceTable = from bg in _context.BangGia
                                          join hd in _context.HopDongVaPhuLuc
                                          on bg.MaHopDong equals hd.MaHopDong
                                          where (hd.MaKh == transport.MaKh || hd.MaKh == item.DonViVanTai)
                                          && bg.MaCungDuong == transport.MaCungDuong
                                          && bg.TrangThai == 4
                                          && bg.NgayApDung.Date <= DateTime.Now.Date
                                          && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                                          && bg.MaDvt == item.MaDVT
                                          && bg.MaLoaiHangHoa == item.MaLoaiHangHoa
                                          && bg.MaLoaiPhuongTien == item.MaLoaiPhuongTien
                                          && bg.MaPtvc == item.MaPTVC
                                          select bg;

                    if (checkPriceTable.Count() == 2)
                    {
                        if (item.MaLoaiPhuongTien.Contains("TRUCK"))
                        {
                            await _context.DieuPhoi.AddAsync(new DieuPhoi()
                            {
                                MaVanDon = transport.MaVanDon,
                                MaSoXe = item.MaSoXe,
                                MaTaiXe = item.MaTaiXe,
                                DonViVanTai = item.DonViVanTai,
                                MaLoaiHangHoa = item.MaLoaiHangHoa,
                                MaDvt = item.MaDVT,
                                MaLoaiPhuongTien = item.MaLoaiPhuongTien,
                                MaPtvc = item.MaPTVC,
                                HangTau = null,
                                Tau = null,
                                BangGiaNcc = checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").Select(x => x.Id).FirstOrDefault(),
                                DonGiaNcc = checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").Select(x => x.DonGia).FirstOrDefault(),
                                BangGiaKh = checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").Select(x => x.Id).FirstOrDefault(),
                                DonGiaKh = checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").Select(x => x.DonGia).FirstOrDefault(),
                                MaRomooc = null,
                                ContNo = null,
                                SealNp = item.SealNp,
                                SealHq = null,
                                KhoiLuong = item.KhoiLuong,
                                TheTich = item.TheTich,
                                GhiChu = item.GhiChu,
                                DiemLayTraRong = null,

                                TrangThai = 19,
                                CreatedTime = DateTime.Now,
                            });
                        }
                        else
                        {
                            await _context.DieuPhoi.AddAsync(new DieuPhoi()
                            {
                                MaVanDon = transport.MaVanDon,
                                MaSoXe = item.MaSoXe,
                                MaTaiXe = item.MaTaiXe,
                                DonViVanTai = item.DonViVanTai,
                                MaLoaiHangHoa = item.MaLoaiHangHoa,
                                MaDvt = item.MaDVT,
                                MaLoaiPhuongTien = item.MaLoaiPhuongTien,
                                MaPtvc = item.MaPTVC,
                                HangTau = item.HangTau,
                                Tau = item.TenTau,
                                BangGiaNcc = checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").Select(x => x.Id).FirstOrDefault(),
                                DonGiaNcc = checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").Select(x => x.DonGia).FirstOrDefault(),
                                BangGiaKh = checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").Select(x => x.Id).FirstOrDefault(),
                                DonGiaKh = checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").Select(x => x.DonGia).FirstOrDefault(),
                                MaRomooc = item.MaRomooc,
                                ContNo = item.ContNo,
                                SealNp = item.SealNp,
                                SealHq = item.SealHq,
                                KhoiLuong = item.KhoiLuong,
                                TheTich = item.TheTich,
                                GhiChu = item.GhiChu,
                                DiemLayTraRong = item.DiemLayTraRong,

                                TrangThai = 19,
                                CreatedTime = DateTime.Now,
                            });
                        }
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Thùng hàng số " + thung + " chưa có giá chuẩn" };
                    }
                }

                var result = await _context.SaveChangesAsync();
                transaction.Commit();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Điều Phố Vận Đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điều Phố Vận Đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CreateTransport(CreateTransport request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (request.MaCungDuong.Length != 10)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã cung đường không đúng" };
                }

                if (!Regex.IsMatch(request.MaCungDuong, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã cung đường không đúng" };
                }

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

                if (request.LoaiVanDon == "nhap")
                {
                    if (request.ThoiGianHanLenh == null || request.ThoiGianCoMat == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                    }
                }
                else
                {
                    if (request.ThoiGianHaCang == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                    }
                }

                if (request.TongKhoiLuong < 1 || request.TongTheTich < 1 || request.TongThungHang < 1)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tổng khối lượng, tổng thể tích, tổng thùng hàng không được nhỏ hơn 1" };
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
                    MaKh = request.MaKH,
                    MaVanDon = transPortId,
                    MaVanDonKh = request.MaVanDonKH,
                    TongThungHang = request.TongThungHang,
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
                    TrangThai = 8,
                    ThoiGianTaoDon = DateTime.Now,
                    CreatedTime = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    for (int i = 0; i < request.TongThungHang; i++)
                    {
                        await _context.DieuPhoi.AddAsync(new DieuPhoi()
                        {
                            MaVanDon = transPortId,
                            TrangThai = 19,
                            CreatedTime = DateTime.Now,
                        });
                    }

                    var resultDP = await _context.SaveChangesAsync();

                    transaction.Commit();

                    if (resultDP > 0)
                    {
                        return new BoolActionResult { isSuccess = true, Message = "Tạo vận đơn Thành Công!" };
                    }
                    else
                    {
                        transaction.Rollback();
                        return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                    }
                }
                else
                {
                    transaction.Rollback();
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
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
                    TongThungHang = x.transport.TongThungHang,
                    ThoiGianCoMat = x.transport.ThoiGianCoMat,
                    ThoiGianHaCang = x.transport.ThoiGianHaCang,
                    ThoiGianHanLenh = x.transport.ThoiGianHanLenh,
                    ThoiGianLayHang = x.transport.ThoiGianLayHang,
                    ThoiGianTraHang = x.transport.ThoiGianTraHang,
                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BoolActionResult> UpdateTransport(string transPortId, UpdateTransport request)
        {

            var transaction = _context.Database.BeginTransaction();

            try
            {
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

                checkTransport.MaVanDonKh = request.MaVanDonKH;
                checkTransport.MaCungDuong = request.MaCungDuong;
                checkTransport.MaKh = request.MaKh;
                checkTransport.TongKhoiLuong = request.TongKhoiLuong;
                checkTransport.TongTheTich = request.TongTheTich;
                checkTransport.ThoiGianLayHang = request.ThoiGianLayHang;
                checkTransport.ThoiGianTraHang = request.ThoiGianTraHang;
                checkTransport.ThoiGianLayTraRong = request.ThoiGianLayTraRong;
                checkTransport.ThoiGianCoMat = request.ThoiGianCoMat;
                checkTransport.ThoiGianHaCang = request.ThoiGianHaCang;
                checkTransport.ThoiGianHanLenh = request.ThoiGianHanLenh;
                checkTransport.UpdatedTime = DateTime.Now;

                if (checkTransport.TongThungHang != request.TongThungHang)
                {
                    if (checkTransport.TongThungHang > request.TongThungHang)
                    {
                        int num = checkTransport.TongThungHang - request.TongThungHang;

                        var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == transPortId && x.TrangThai == 19).Take(num).ToListAsync();

                        if (getListHandling.Count < num)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "không thể chỉnh lại tổng thùng hàng vì lệnh điều phối đã được thực hiện" };
                        }
                        _context.DieuPhoi.RemoveRange(getListHandling);

                    }
                    else
                    {
                        int num = request.TongThungHang - checkTransport.TongThungHang;

                        for (int i = 0; i < num; i++)
                        {
                            await _context.DieuPhoi.AddAsync(new DieuPhoi()
                            {
                                MaVanDon = transPortId,
                                TrangThai = 19,
                                CreatedTime = DateTime.Now,
                            });
                        }
                    }
                    checkTransport.TongThungHang = request.TongThungHang;
                }

                _context.VanDon.Update(checkTransport);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    transaction.Commit();
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<PagedResponseCustom<ListTransport>> GetListTransport(PaginationFilter filter)
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
                           where status.LangId == TempData.LangID
                           orderby transport.ThoiGianTaoDon descending
                           select new { transport, status, road, kh };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.transport.MaVanDon.Contains(filter.Keyword));
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

            var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListTransport()
            {
                MaVanDon = x.transport.MaVanDon,
                MaKH = x.transport.MaKh,
                TenKH = x.kh.TenKh,
                LoaiVanDon = x.transport.LoaiVanDon,
                MaCungDuong = x.road.MaCungDuong,
                TenCungDuong = x.road.TenCungDuong,
                DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                TongKhoiLuong = x.transport.TongKhoiLuong,
                TongTheTich = x.transport.TongTheTich,

            }).ToListAsync();

            return new PagedResponseCustom<ListTransport>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<PagedResponseCustom<ListHandling>> GetListHandling(string transportId, PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var listData = from vd in _context.VanDon
                           join
                           dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon
                           join bg in _context.BangGia
                           on dp.BangGiaKh equals bg.Id
                           join tt in _context.StatusText
                           on dp.TrangThai equals tt.StatusId
                           where tt.LangId == TempData.LangID
                           select new { vd, dp, bg, tt };

            if (!string.IsNullOrEmpty(transportId))
            {
                listData = listData.Where(x => x.dp.MaVanDon == transportId);
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.dp.MaSoXe.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.statusId))
            {
                listData = listData.Where(x => x.tt.StatusId == int.Parse(filter.statusId));
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                listData = listData.Where(x => x.vd.ThoiGianTaoDon.Date >= filter.fromDate.Value.Date && x.vd.ThoiGianTaoDon.Date <= filter.toDate.Value.Date);
            }

            var getListHandlingNew = from vd in _context.VanDon
                                     join dp in _context.DieuPhoi
                                     on vd.MaVanDon equals dp.MaVanDon
                                     join tt in _context.StatusText
                                     on dp.TrangThai equals tt.StatusId
                                     where tt.LangId == TempData.LangID
                                     && tt.StatusId == 19
                                     select new { vd, dp, tt };

            var listHandlingNew = await getListHandlingNew.Select(x => new ListHandling()
            {
                CungDuong = _context.CungDuong.Where(y => y.MaCungDuong == x.vd.MaCungDuong).Select(x => x.TenCungDuong).FirstOrDefault(),
                MaVanDon = x.dp.MaVanDon,
                PhanLoaiVanDon = x.vd.LoaiVanDon,
                MaDieuPhoi = x.dp.Id,
                DiemLayRong = "",
                MaSoXe = "",
                TenTaiXe = "",
                SoDienThoai = "",
                PTVanChuyen = "",
                TenTau = "",
                HangTau = "",
                MaRomooc = "",
                ContNo = "",
                KhoiLuong = null,
                TheTich = null,
                TrangThai = x.tt.StatusContent,
                statusId = x.tt.StatusId,
            }).ToListAsync();

            var listHandling = await listData.Select(x => new ListHandling()
            {
                CungDuong = _context.CungDuong.Where(y => y.MaCungDuong == x.vd.MaCungDuong).Select(x => x.TenCungDuong).FirstOrDefault(),
                MaVanDon = x.dp.MaVanDon,
                PhanLoaiVanDon = x.vd.LoaiVanDon,
                MaDieuPhoi = x.dp.Id,
                DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                MaSoXe = x.dp.MaSoXe,
                TenTaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(y => y.HoVaTen).FirstOrDefault(),
                SoDienThoai = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(y => y.SoDienThoai).FirstOrDefault(),
                PTVanChuyen = x.bg.MaLoaiPhuongTien,
                TenTau = x.dp.Tau,
                HangTau = x.dp.HangTau,
                MaRomooc = x.dp.MaRomooc,
                ContNo = x.dp.ContNo,
                KhoiLuong = x.dp.KhoiLuong,
                TheTich = x.dp.TheTich,
                TrangThai = x.tt.StatusContent,
                statusId = x.tt.StatusId
            }).ToListAsync();

            var data = listHandlingNew.Concat(listHandling);

            var totalCount = data.Count();

            var pagedData = data.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).OrderByDescending(x => x.MaVanDon);

            return new PagedResponseCustom<ListHandling>()
            {
                dataResponse = pagedData.OrderByDescending(x => x.MaVanDon).ToList(),
                totalCount = totalCount,
                paginationFilter = validFilter
            };
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
                    MaPTVC = data.dp.MaPtvc,
                    MaDVT = data.dp.MaDvt,
                    MaKh = data.vd.MaKh,
                    MaVanDon = data.vd.MaVanDon,
                    MaCungDuong = data.vd.MaCungDuong,
                    MaSoXe = data.dp.MaSoXe,
                    MaTaiXe = data.dp.MaTaiXe,
                    DonViVanTai = data.dp.DonViVanTai,
                    PTVanChuyen = data.dp.MaLoaiPhuongTien,
                    TenTau = data.dp.Tau,
                    HangTau = data.dp.HangTau,
                    MaRomooc = data.dp.MaRomooc,
                    DiemLayRong = data.dp.DiemLayTraRong,
                    ContNo = data.dp.ContNo,
                    SealNp = data.dp.SealNp,
                    SealHq = data.dp.SealHq,
                    KhoiLuong = data.dp.KhoiLuong,
                    TheTich = data.dp.TheTich,
                    GhiChu = data.dp.GhiChu,
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

                //string errs = "";

                //if (handling.MaLoaiPhuongTien.Contains("CONT"))
                //{
                //    if (handling.ThoiGianLayTraRong == null)
                //    {
                //        errs += "Vui lòng không để trống thời gian lấy/trả rỗng";
                //    }

                //    if (string.IsNullOrEmpty(handling.ContNo))
                //    {
                //        errs += "Vui Lòng không để trống ContNo";
                //    }

                //    if (getTransport.LoaiVanDon == "xuat")
                //    {
                //        if (string.IsNullOrEmpty(handling.HangTau) || string.IsNullOrEmpty(handling.Tau))
                //        {
                //            errs += "Vui lòng không để trống thông tin Tàu";
                //        }

                //        if (handling.ThoiGianCatMang == null)
                //        {
                //            errs += "Vui lòng không để trống thời gian cắt máng";
                //        }
                //    }
                //}

                //if (!string.IsNullOrEmpty(errs))
                //{
                //    return new BoolActionResult { isSuccess = false, Message = errs };
                //}

                if (handling.TrangThai == 18)
                {
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
                                return new BoolActionResult { isSuccess = true, Message = "Mã vận đơn " + handling.MaVanDon + " đã hoàn thành" };
                            }
                            else
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                            }
                        }

                        return new BoolActionResult { isSuccess = true, Message = "Tài Xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " và Xe: " + handling.MaSoXe + " đã hoàn thành chuyến" };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }

                if (handling.MaLoaiPhuongTien.Contains("CONT") && handling.TrangThai == 17)
                {
                    handling.TrangThai = 18;
                    _context.Update(handling);

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        return new BoolActionResult { isSuccess = true, Message = "Đã điều phối xe: " + handling.MaSoXe + " và tài xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " vận chuyển hàng" };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }

                if (handling.TrangThai == 19)
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
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CancelHandling(int id)
        {
            try
            {
                var getByid = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (getByid == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Lệnh điều phối không tồn tại" };
                }

                if (getByid.TrangThai != 19)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể hủy lệnh điều phối này " };
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

                    return new BoolActionResult { isSuccess = true, Message = "Đã hủy lệnh điều phối thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hủy lệnh thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UpdateHandling(int id, UpdateHandling request)
        {
            try
            {
                if (request.KhoiLuong == null || request.TheTich == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không được để trống Khối Lượng, Thể Tích" };
                }

                if (string.IsNullOrEmpty(request.SealNp))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không được để trống  SealNP" };
                }

                //if (request.PTVanChuyen.Contains("Cont"))
                //{
                //    if (string.IsNullOrEmpty(request.ContNo) || string.IsNullOrEmpty(request.SealHq))
                //    {
                //        return new BoolActionResult { isSuccess = false, Message = "Không được để trống ContNo, Seal HQ" };
                //    }

                //    if (string.IsNullOrEmpty(request.ThoiGianLayRong.ToString()) || string.IsNullOrEmpty(request.ThoiGianKeoCong.ToString())
                //  || string.IsNullOrEmpty(request.ThoiGianHanLenh.ToString()) || string.IsNullOrEmpty(request.ThoiGianCoMat.ToString())
                //  || string.IsNullOrEmpty(request.ThoiGianTraRong.ToString()) || string.IsNullOrEmpty(request.ThoiGianLayHang.ToString())
                //  || string.IsNullOrEmpty(request.ThoiGianTraHang.ToString()))
                //    {
                //        return new BoolActionResult { isSuccess = false, Message = "Không được để trống các trường thời gian" };
                //    }
                //}
                //else
                //{
                //    if (string.IsNullOrEmpty(request.ThoiGianCoMat.ToString())
                // || string.IsNullOrEmpty(request.ThoiGianLayHang.ToString())
                // || string.IsNullOrEmpty(request.ThoiGianTraHang.ToString()))
                //    {
                //        return new BoolActionResult { isSuccess = false, Message = "Không được để trống các trường thời gian" };
                //    }
                //}

                var checkById = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (checkById == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã điều phối không tồn tại" };
                }

                var transport = await _context.VanDon.Where(x => x.MaVanDon == checkById.MaVanDon).FirstOrDefaultAsync();

                if (request.PTVanChuyen.Contains("Cont"))
                {
                    if (request.ThoiGianLayTraRong == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không được để trống thời gian lấy/trả rỗng \r\n" };
                    }

                    if (transport.LoaiVanDon == "xuat")
                    {
                        if (string.IsNullOrEmpty(request.HangTau) || string.IsNullOrEmpty(request.TenTau))
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không được để trống hãng tàu và tên tàu \r\n" };
                        }

                        if (request.ThoiGianCatMang == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không được để trống thời gian cắt máng \r\n" };
                        }
                    }
                }

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

                if (checkVehicle.MaLoaiPhuongTien != request.PTVanChuyen)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại xe không khớp với xe vận chuyển \r\n" };
                }

                checkById.MaSoXe = request.MaSoXe;
                checkById.MaTaiXe = request.MaTaiXe;
                checkById.SealNp = request.SealNp;
                checkById.KhoiLuong = request.KhoiLuong;
                checkById.TheTich = request.TheTich;
                checkById.GhiChu = request.GhiChu;


                var checkTransportType = await _context.VanDon.Where(x => x.MaVanDon == checkById.MaVanDon).FirstOrDefaultAsync();
                if (checkTransportType.LoaiVanDon == "xuat")
                {
                    checkById.Tau = request.TenTau;
                    checkById.HangTau = request.HangTau;

                }

                var checkVehicleType = await _context.BangGia.Where(x => x.Id == checkById.BangGiaKh).FirstOrDefaultAsync();
                if (checkVehicleType.MaLoaiPhuongTien.Contains("CONT"))
                {
                    checkById.MaRomooc = request.MaRomooc;
                    checkById.ContNo = request.ContNo;
                    checkById.SealHq = request.SealHq;

                }

                _context.DieuPhoi.Update(checkById);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = true, Message = ex.ToString() };
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

            return new BoolActionResult { isSuccess = true, Message = "Upload hình ảnh thành công" };
        }
    }
}