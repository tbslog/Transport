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
            var transaction = await _context.Database.BeginTransactionAsync();
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

                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (request.LoaiThungHang.Contains("CONT"))
                {
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

                    if (request.ThoiGianLayTraRong == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian lấy/trả rỗng" };
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

                if (request.LoaiVanDon == "nhap")
                {
                    if (!request.LoaiThungHang.Contains("CONT"))
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
                    if (!request.LoaiThungHang.Contains("CONT"))
                    {
                        request.ThoiGianHaCang = null;
                        request.ThoiGianLayTraRong = null;
                    }
                }

                await _context.VanDon.AddRangeAsync(new VanDon()
                {
                    LoaiThungHang = request.LoaiThungHang,
                    MaKh = request.MaKH,
                    HangTau = request.HangTau,
                    Tau = request.TenTau,
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

                    await transaction.CommitAsync();

                    if (resultDP > 0)
                    {
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
                    LoaiThungHang = x.transport.LoaiThungHang,
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
                    ThoiGianTaoDon = x.transport.ThoiGianTaoDon,
                    HangTau = x.transport.HangTau,
                    TenTau = x.transport.Tau
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

                if (request.LoaiThungHang.Contains("CONT"))
                {
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

                    if (request.ThoiGianLayTraRong == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian lấy/trả rỗng" };
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
                    if (!request.LoaiThungHang.Contains("CONT"))
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
                    if (!request.LoaiThungHang.Contains("CONT"))
                    {
                        request.ThoiGianHaCang = null;
                        request.ThoiGianLayTraRong = null;
                    }
                }

                checkTransport.LoaiThungHang = request.LoaiThungHang;
                checkTransport.HangTau = request.HangTau;
                checkTransport.Tau = request.TenTau;
                checkTransport.MaVanDonKh = request.MaVanDonKH;
                checkTransport.MaCungDuong = request.MaCungDuong;
                checkTransport.MaKh = request.MaKH;
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
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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
                ThoiGianLayHang = x.transport.ThoiGianLayHang,
                ThoiGianTaoDon = x.transport.ThoiGianTaoDon,
                ThoiGianTraHang = x.transport.ThoiGianTraHang,
                ThoiGianCoMat = x.transport.ThoiGianCoMat,
                ThoiGianLayTraRong = x.transport.ThoiGianLayTraRong,
                ThoiGianHanLenh = x.transport.ThoiGianHanLenh,
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

        public async Task<PagedResponseCustom<ListHandling>> GetListHandling(string transportId, PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var listData = from vd in _context.VanDon
                           join
                           dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon
                           join tt in _context.StatusText
                           on dp.TrangThai equals tt.StatusId
                           join bg in _context.BangGia
                           on dp.BangGiaKh equals bg.Id into bgdp
                           from bgvd in bgdp.DefaultIfEmpty()
                           where tt.LangId == TempData.LangID
                           select new { vd, dp, tt, bgvd };

            if (!string.IsNullOrEmpty(transportId))
            {
                listData = listData.Where(x => x.vd.MaVanDon == transportId);
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.dp.MaSoXe.Contains(filter.Keyword) || x.dp.ContNo.Contains(filter.Keyword));
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

            var pagedData = await listData.OrderByDescending(x => x.vd.MaVanDon).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListHandling()
            {
                CungDuong = _context.CungDuong.Where(y => y.MaCungDuong == x.vd.MaCungDuong).Select(x => x.TenCungDuong).FirstOrDefault(),
                MaVanDon = x.dp.MaVanDon,
                PhanLoaiVanDon = x.vd.LoaiVanDon,
                MaDieuPhoi = x.dp.Id,
                DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                MaSoXe = x.dp.MaSoXe,
                TenTaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(y => y.HoVaTen).FirstOrDefault(),
                SoDienThoai = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(y => y.SoDienThoai).FirstOrDefault(),
                PTVanChuyen = x.bgvd.MaLoaiPhuongTien,
                MaRomooc = x.dp.MaRomooc,
                ContNo = x.dp.ContNo,
                KhoiLuong = x.dp.KhoiLuong,
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
                    TenTau = data.vd.Tau,
                    HangTau = data.vd.HangTau,
                    MaRomooc = data.dp.MaRomooc,
                    DiemLayRong = data.dp.DiemLayTraRong,
                    ContNo = data.dp.ContNo,
                    SealNp = data.dp.SealNp,
                    SealHq = data.dp.SealHq,
                    KhoiLuong = data.dp.KhoiLuong,
                    TheTich = data.dp.TheTich,
                    GhiChu = data.dp.GhiChu,

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
                    if (handling.ThoiGianTraHangThucTe == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Trả Hàng thực tế " };

                    }
                    if (handling.ThoiGianLayHangThucTe == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Lấy Hàng thực tế " };
                    }

                    if (handling.MaLoaiPhuongTien.Contains("CONT"))
                    {
                        if (handling.ThoiGianLayTraRongThucTe == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Lấy/Trả rỗng thực tế " };
                        }
                    }


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
                    if (string.IsNullOrEmpty(handling.ContNo))
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật ContNo trước đã" };
                    }
                    if (handling.KhoiLuong == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Khối Lượng trước đã" };
                    }
                    if (handling.TheTich == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thể Tích trước đã" };
                    }


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

                if (getByid.TrangThai != 19 || getByid.TrangThai == 27)
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

                    if (getAllHandlingOfTransport.Where(x => x.TrangThai == 20 || x.TrangThai == 21).Count() == getAllHandlingOfTransport.Count())
                    {
                        var getTransport = await _context.VanDon.Where(x => x.MaVanDon == getByid.MaVanDon).FirstOrDefaultAsync();
                        getTransport.TrangThai = 22;
                        getTransport.ThoiGianHoanThanh = DateTime.Now;
                        _context.Update(getTransport);

                        var result3 = await _context.SaveChangesAsync();
                        if (result > 0)
                        {
                            return new BoolActionResult { isSuccess = true, Message = "Mã vận đơn " + getTransport.MaVanDon + " đã hoàn thành" };
                        }
                        else
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
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
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var checkById = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (checkById == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã điều phối không tồn tại" };
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
                var checkPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == request.MaPtvc).Select(x => x.TenPtvc).FirstOrDefaultAsync();
                if (checkPTVC == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Phương Thức Vận Chuyển Không Tồn Tại" };
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

                if (!getTransport.LoaiThungHang.Contains("CONT") && request.PTVanChuyen.Contains("CONT"))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vận đơn này chỉ được điều xe Truck" };
                }

                if (request.PTVanChuyen.Contains("CONT"))
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

                var checkPriceTable = from bg in _context.BangGia
                                      join hd in _context.HopDongVaPhuLuc
                                      on bg.MaHopDong equals hd.MaHopDong
                                      where (hd.MaKh == getTransport.MaKh || hd.MaKh == request.DonViVanTai)
                                      && bg.MaCungDuong == getTransport.MaCungDuong
                                      && bg.TrangThai == 4
                                      && bg.NgayApDung.Date <= DateTime.Now.Date
                                      && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                                      && bg.MaDvt == request.DonViTinh
                                      && bg.MaLoaiHangHoa == request.LoaiHangHoa
                                      && bg.MaLoaiPhuongTien == request.PTVanChuyen
                                      && bg.MaPtvc == request.MaPtvc
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
                            + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                        + " chưa có bảng giá cho Cung Đường: " + getTransport.MaCungDuong +
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
                           + await _context.KhachHang.Where(x => x.MaKh == getTransport.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                        + " chưa có bảng giá cho Cung Đường: " + getTransport.MaCungDuong +
                        ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                        ", Loại Hàng Hóa:" + checkGoodsType +
                        ", Đơn Vị Tính: " + checkDVT +
                        ", Phương thức vận chuyển: " + checkPTVC
                        };
                    }

                    if (!request.PTVanChuyen.Contains("CONT"))
                    {
                        request.MaRomooc = null;
                        request.ContNo = null;
                        request.SealHq = null;
                        request.DiemLayTraRong = null;
                        request.ThoiGianLayTraRongThucTe = null;
                        request.ThoiGianCoMatThucTe = null;
                        request.ThoiGianHaCangThucTe = null;
                    }

                    if (getTransport.LoaiVanDon == "xuat")
                    {
                        request.ThoiGianCoMatThucTe = null;
                    }
                    else
                    {
                        request.ThoiGianHaCangThucTe = null;
                    }

                    if (checkById.TrangThai == 17 || checkById.TrangThai == 18 || checkById.TrangThai == 20)
                    {
                        checkById.ThoiGianLayHangThucTe = request.ThoiGianLayHangThucTe;
                        checkById.ThoiGianTraHangThucTe = request.ThoiGianTraHangThucTe;
                        checkById.ThoiGianLayTraRongThucTe = request.ThoiGianLayTraRongThucTe;
                        checkById.ThoiGianCoMatThucTe = request.ThoiGianCoMatThucTe;
                        checkById.KhoiLuong = request.KhoiLuong;
                        checkById.TheTich = request.TheTich;
                        checkById.ContNo = request.ContNo;
                        checkById.SealHq = request.SealHq;
                        checkById.SealNp = request.SealNp;
                        checkById.MaSoXe = request.MaSoXe;
                        checkById.MaTaiXe = request.MaTaiXe;
                        checkById.MaRomooc = request.MaRomooc;
                    }

                    if (checkById.TrangThai == 19 || checkById.TrangThai == 27)
                    {
                        checkById.MaSoXe = request.MaSoXe;
                        checkById.MaTaiXe = request.MaTaiXe;
                        checkById.MaLoaiHangHoa = request.LoaiHangHoa;
                        checkById.MaPtvc = request.MaPtvc;
                        checkById.MaLoaiPhuongTien = request.PTVanChuyen;
                        checkById.MaDvt = request.DonViTinh;
                        checkById.DonViVanTai = request.DonViVanTai;
                        checkById.BangGiaKh = priceTableCustomer.Id;
                        checkById.BangGiaNcc = priceTableSupplier.Id;
                        checkById.DonGiaKh = priceTableCustomer.DonGia;
                        checkById.DonGiaNcc = priceTableSupplier.DonGia;
                        checkById.MaRomooc = request.MaRomooc;
                        checkById.KhoiLuong = request.KhoiLuong;
                        checkById.TheTich = request.TheTich;
                        checkById.GhiChu = request.GhiChu;
                        checkById.DiemLayTraRong = request.DiemLayTraRong;
                        checkById.TrangThai = 27;
                        getTransport.TrangThai = 9;
                    }
                    _context.Update(getTransport);
                    _context.Update(checkById);

                    var result = await _context.SaveChangesAsync();

                    if (result > 0)
                    {
                        await transaction.CommitAsync();
                        return new BoolActionResult { isSuccess = true, Message = "Điều Phối Chuyến Thành Công!" };
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return new BoolActionResult { isSuccess = false, Message = "Điều Phối Chuyến Thất Bại" };
                    }
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
                            + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                        + " chưa có bảng giá cho Cung Đường: " + getTransport.MaCungDuong +
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
                           + await _context.KhachHang.Where(x => x.MaKh == getTransport.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                        + " chưa có bảng giá cho Cung Đường: " + getTransport.MaCungDuong +
                        ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                        ", Loại Hàng Hóa:" + checkGoodsType +
                        ", Đơn Vị Tính: " + checkDVT +
                        ", Phương thức vận chuyển: " + checkPTVC
                        };
                    }

                    return new BoolActionResult { isSuccess = false, Message = "Không có bảng giá khách hàng lẫn nhà cung cấp" };
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

            return new BoolActionResult { isSuccess = true, Message = "Upload hình ảnh thành công" };
        }
    }
}