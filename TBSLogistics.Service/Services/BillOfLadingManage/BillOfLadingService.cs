using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.CustomerModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
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

        public async Task<LoadDataHandling> LoadDataHandling(string RoadId)
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

            var listRomooc = from rm in _context.Romooc join lrm in _context.LoaiRomooc on rm.MaLoaiRomooc equals lrm.MaLoaiRomooc select new { rm, lrm };

            var result = new LoadDataHandling()
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
                var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();

                if (checkRoad == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cung đường không tồn tại" };
                }

                string ErrorValidate = "";
                foreach (var item in request.DieuPhoi)
                {
                    if (string.IsNullOrEmpty(item.DonViVanTai))
                    {
                        ErrorValidate += "Không được để trống Đơn vị vận tải";
                    }

                    if (item.DonViVanTai.Length != 8)
                    {
                        ErrorValidate += "Đơn vị vận tải phải dài 8 ký tự \r\n" + System.Environment.NewLine;
                    }

                    if (!Regex.IsMatch(item.DonViVanTai, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
                    {
                        ErrorValidate += "Đơn vị vận tải không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
                    }

                    if (string.IsNullOrEmpty(item.MaKh))
                    {
                        ErrorValidate += "Không được để trống đơn vị vận tải";
                    }

                    if (item.MaKh.Length != 8)
                    {
                        ErrorValidate += "Mã khách hàng phải dài 8 ký tự \r\n" + System.Environment.NewLine;
                    }

                    if (!Regex.IsMatch(item.MaKh, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
                    {
                        ErrorValidate += "Mã khách hàng không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
                    }

                    if (string.IsNullOrEmpty(item.GiaThamChieu.ToString()) || string.IsNullOrEmpty(item.GiaThucTe.ToString()))
                    {
                    }

                    if (!Regex.IsMatch(item.GiaThamChieu.ToString(), "^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$") || !Regex.IsMatch(item.GiaThucTe.ToString(), "^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$"))
                    {
                    }

                    if (string.IsNullOrEmpty(item.ContNo))
                    {
                    }

                    if (string.IsNullOrEmpty(item.MaTaiXe) || string.IsNullOrEmpty(item.MaSoXe))
                    {
                    }
                }

                var transport = await _context.VanDon.Where(x => x.MaVanDon == request.MaVanDon).FirstOrDefaultAsync();

                if (transport == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn không tồn tại" };
                }

                transport.TrangThai = 9;
                _context.VanDon.Update(transport);

                var handling = request.DieuPhoi.Select(x => new DieuPhoi()
                {
                    MaVanDon = request.MaVanDon,
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
                    KhoiLuong = x.KhoiLuong,
                    TheTich = x.TheTich,
                    GhiChu = x.GhiChu,
                    ThoiGianLayRong = x.ThoiGianLayRong,
                    ThoiGianHaCong = x.ThoiGianHaCong,
                    ThoiGianKeoCong = x.ThoiGianKeoCong,
                    ThoiGianHanLenh = x.ThoiGianHanLenh,
                    ThoiGianCoMat = x.ThoiGianCoMat,
                    ThoiGianCatMang = x.ThoiGianCatMang,
                    ThoiGianTraRong = x.ThoiGianTraRong,
                    ThoiGianNhapHang = x.ThoiGianNhapHang,
                    ThoiGianXuatHang = x.ThoiGianXuatHang,
                    TrangThai = 9,
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

        public async Task<BoolActionResult> CreateTransport(CreateTransport request)
        {
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

                if (request.TongThungHang < 1 || request.TongKhoiLuong < 1)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tổng thùng hàng phải lớn hơn 0, tổng trọng lượng phải lớn hơn 0" };
                }

                var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();
                if (checkRoad == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cung đường không tồn tại" };
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
                    LoaiVanDon = request.LoaiVanDon,
                    MaCungDuong = request.MaCungDuong,
                    TongThungHang = request.TongThungHang,
                    TongKhoiLuong = request.TongKhoiLuong,
                    ThoiGianLayHang = request.ThoiGianLayHang,
                    ThoiGianTraHang = request.ThoiGianTraHang,
                    TrangThai = 8,
                    ThoiGianTaoDon = DateTime.Now,
                    CreatedTime = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Tạo vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                }
            }
            catch (Exception ex)
            {
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
                    MaVanDon = x.transport.MaVanDon,
                    LoaiVanDon = x.transport.LoaiVanDon,
                    CungDuong = x.road.MaCungDuong,
                    DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemLayRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    TongThungHang = x.transport.TongThungHang,
                    TongTrongLuong = x.transport.TongKhoiLuong,
                    ThoiGianLayHang = x.transport.ThoiGianLayHang,
                    ThoiGianTraHang = x.transport.ThoiGianTraHang,
                    ThoiGianTaoDon = x.transport.ThoiGianTaoDon,
                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<BoolActionResult> UpdateTransport(string transPortId, UpdateTransport request)
        {
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

                if(checkTransport.TrangThai != 8)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vận đơn này không thể sửa nữa" };
                }

                checkTransport.MaCungDuong = request.MaCungDuong;
                checkTransport.TongThungHang = request.TongThungHang;
                checkTransport.TongKhoiLuong = request.TongKhoiLuong;
                checkTransport.ThoiGianLayHang = request.ThoiGianLayHang;
                checkTransport.ThoiGianTraHang = request.ThoiGianTraHang;

                _context.VanDon.Update(checkTransport);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                }
            }
            catch (Exception ex)
            {
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
                           where status.LangId == TempData.LangID
                           orderby transport.ThoiGianTaoDon descending
                           select new { transport, status, road };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.transport.MaVanDon.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                listData = listData.Where(x => x.status.StatusId == int.Parse(filter.Status));
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                listData = listData.Where(x => x.transport.ThoiGianTaoDon.Date >= filter.fromDate.Value.Date && x.transport.ThoiGianTaoDon.Date <= filter.toDate.Value.Date);
            }

            var totalCount = await listData.CountAsync();

            var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListTransport()
            {
                MaVanDon = x.transport.MaVanDon,
                LoaiVanDon = x.transport.LoaiVanDon,
                MaCungDuong = x.road.MaCungDuong,
                TenCungDuong = x.road.TenCungDuong,
                DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemLayRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
                DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.road.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                TongThungHang = x.transport.TongThungHang,
                TongKhoiLuong = x.transport.TongKhoiLuong,
                ThoiGianLayHang = x.transport.ThoiGianLayHang,
                ThoiGianTraHang = x.transport.ThoiGianTraHang,
                TrangThai = x.status.StatusContent,
                ThoiGianTaoDon = x.transport.ThoiGianTaoDon,
            }).ToListAsync();

            return new PagedResponseCustom<ListTransport>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };

        }
    }
}