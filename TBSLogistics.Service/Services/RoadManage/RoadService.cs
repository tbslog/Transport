using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.RoadManage;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.RoadManage
{
    public class RoadService : IRoad
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public RoadService(ICommon common, TMSContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _common = common;
            _context = context;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }

        public async Task<GetRoadRequest> GetRoadById(string MaCungDuong)
        {
            try
            {
                var getById = await _context.CungDuong.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

                if (getById != null)
                {
                    return new GetRoadRequest()
                    {
                        MaCungDuong = getById.MaCungDuong,
                        TenCungDuong = getById.TenCungDuong,
                        Km = getById.Km,
                        DiemDau = getById.DiemDau,
                        DiemCuoi = getById.DiemCuoi,
                        GhiChu = getById.GhiChu,
                        TrangThai = getById.TrangThai,
                    };
                }

                return new GetRoadRequest();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BoolActionResult> CreateRoad(CreateRoadRequest request)
        {
            try
            {
                string checkValidate = await ValiateRoad(request.TenCungDuong, request.Km, request.DiemDau, request.DiemCuoi, request.GhiChu, "Create");
                if (!string.IsNullOrEmpty(checkValidate))
                {
                    return new BoolActionResult { isSuccess = false, Message = checkValidate };
                }

                var getMaxRoad = await _context.CungDuong.OrderByDescending(x => x.MaCungDuong).Select(x => x.MaCungDuong).FirstOrDefaultAsync();

                string RoadId = "";

                if (string.IsNullOrEmpty(getMaxRoad))
                {
                    RoadId = "CD00000001";
                }
                else
                {
                    RoadId = "CD" + (int.Parse(getMaxRoad.Substring(2, getMaxRoad.Length - 2)) + 1).ToString("00000000");
                }

                await _context.AddAsync(new CungDuong()
                {
                    MaCungDuong = RoadId,
                    TenCungDuong = request.TenCungDuong,
                    Km = request.Km,
                    DiemDau = request.DiemDau,
                    DiemCuoi = request.DiemCuoi,
                    GhiChu = request.GhiChu,
                    TrangThai = request.TrangThai,
                    UpdatedTime = DateTime.Now,
                    CreatedTime = DateTime.Now,
                    Creator = tempData.UserName,
                });

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await _common.Log("RoadManage", "UserId: " + tempData.UserName + " create new Road with Data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới cung đường thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới cung đường thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("RoadManage", "UserId:" + tempData.UserID + " create new Road with Error: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UpdateRoad(string MaCungDuong, UpdateRoadRequest request)
        {
            try
            {
                var checkExists = await _context.CungDuong.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

                if (checkExists == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã cung đường không tồn tại" };
                }

                var checkValidate = await ValiateRoad(request.TenCungDuong, request.Km, request.DiemDau, request.DiemCuoi, request.GhiChu, "Update");
                if (!string.IsNullOrEmpty(checkValidate))
                {
                    return new BoolActionResult { isSuccess = false, Message = checkValidate };
                }

                checkExists.TenCungDuong = request.TenCungDuong;
                checkExists.Km = request.Km;
                //checkExists.DiemDau = request.DiemDau;
                //checkExists.DiemCuoi = request.DiemCuoi;
                checkExists.GhiChu = request.GhiChu;
                checkExists.UpdatedTime = DateTime.Now;
                checkExists.TrangThai = request.TrangThai;
                checkExists.Updater = tempData.UserName;
                _context.CungDuong.Update(checkExists);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await _common.Log("RoadManage", "UserId: " + tempData.UserName + " update Road with Data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật cung đường thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật cung đường thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<PagedResponseCustom<ListRoadRequest>> GetListRoad(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var getData = from cungduong in _context.CungDuong
                              orderby cungduong.UpdatedTime descending
                              select new { cungduong };

                var getAddress = from diadiem in _context.DiaDiem
                                 join phanloaidd in _context.LoaiDiaDiem
                                 on diadiem.MaLoaiDiaDiem equals phanloaidd.MaLoaiDiaDiem

                                 select new { diadiem, phanloaidd };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getData = getData.Where(x => x.cungduong.MaCungDuong.ToLower().Contains(filter.Keyword.ToLower()));
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    getData = getData.Where(x => x.cungduong.CreatedTime.Date >= filter.fromDate && x.cungduong.CreatedTime <= filter.toDate);
                }

                var totalRecords = await getData.CountAsync();

                var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListRoadRequest()
                {
                    MaCungDuong = x.cungduong.MaCungDuong,
                    TenCungDuong = x.cungduong.TenCungDuong,
                    Km = x.cungduong.Km,
                    DiemDau = getAddress.Where(y => y.diadiem.MaDiaDiem == x.cungduong.DiemDau).Select(y => y.diadiem.TenDiaDiem).FirstOrDefault(),
                    DiemCuoi = getAddress.Where(y => y.diadiem.MaDiaDiem == x.cungduong.DiemCuoi).Select(y => y.diadiem.TenDiaDiem).FirstOrDefault(),
                    GhiChu = x.cungduong.GhiChu,
                    PhanLoaiDiaDiem = getAddress.Select(x => x.phanloaidd.TenPhanLoaiDiaDiem).FirstOrDefault(),
                    TrangThai = x.cungduong.TrangThai
                }).ToListAsync();

                return new PagedResponseCustom<ListRoadRequest>()
                {
                    paginationFilter = validFilter,
                    totalCount = totalRecords,
                    dataResponse = pagedData
                };
            }
            catch (Exception ex)
            {
                return new PagedResponseCustom<ListRoadRequest>();
            }
        }

        //public async Task<BoolActionResult> ImportExcel(IFormFile formFile, CancellationToken cancellationToken)
        //{
        //    int ErrorRow = 0;
        //    int ErrorInsert = 1;

        //    string ErrorValidate = "";

        //    try
        //    {
        //        if (formFile == null || formFile.Length <= 0)
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
        //        }

        //        if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
        //        }

        //        var list = new List<CreateRoadRequest>();

        //        using (var stream = new MemoryStream())
        //        {
        //            await formFile.CopyToAsync(stream, cancellationToken);

        //            using (var package = new ExcelPackage(stream))
        //            {
        //                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //                var rowCount = worksheet.Dimension.Rows;

        //                if (rowCount == 0)
        //                {
        //                    return new BoolActionResult { isSuccess = false, Message = "This file is empty" };
        //                }

        //                if (worksheet.Cells[1, 1].Value.ToString().Trim() != "Mã cung đường" ||
        //                    worksheet.Cells[1, 2].Value.ToString().Trim() != "Tên cung đường" ||
        //                    worksheet.Cells[1, 3].Value.ToString().Trim() != "Mã hợp đồng" ||
        //                    worksheet.Cells[1, 4].Value.ToString().Trim() != "Số KM" ||
        //                    worksheet.Cells[1, 5].Value.ToString().Trim() != "Điểm đầu" ||
        //                    worksheet.Cells[1, 6].Value.ToString().Trim() != "Điểm cuối" ||
        //                    worksheet.Cells[1, 7].Value.ToString().Trim() != "Điểm lấy rỗng" ||
        //                    worksheet.Cells[1, 8].Value.ToString().Trim() != "Ghi Chú"
        //                    )
        //                {
        //                    return new BoolActionResult { isSuccess = false, Message = "File excel không đúng " };
        //                }

        //                for (int row = 2; row <= rowCount; row++)
        //                {
        //                    ErrorRow = row;

        //                    int? DiemLayRong;
        //                    string MaCungDuong = worksheet.Cells[row, 1].Value.ToString().Trim().ToUpper();
        //                    string TenCungDuong = worksheet.Cells[row, 2].Value.ToString().Trim();

        //                    double SoKM = double.Parse(worksheet.Cells[row, 4].Value.ToString().Trim());
        //                    int DiemDau = int.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
        //                    int DiemCuoi = int.Parse(worksheet.Cells[row, 6].Value.ToString().Trim());
        //                    if (!string.IsNullOrEmpty(worksheet.Cells[row, 7].Value.ToString().Trim()))
        //                    {
        //                        DiemLayRong = int.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
        //                    }
        //                    else
        //                    {
        //                        DiemLayRong = null;
        //                    }
        //                    string GhiChu = worksheet.Cells[row, 8].Value.ToString().Trim();

        //                    ErrorValidate = await ValiateRoad(MaCungDuong, TenCungDuong, SoKM, DiemDau, DiemCuoi, GhiChu, ErrorRow.ToString());

        //                    if (ErrorValidate == "")
        //                    {
        //                        list.Add(new CreateRoadRequest
        //                        {
        //                            MaCungDuong = MaCungDuong,
        //                            TenCungDuong = TenCungDuong,
        //                            Km = SoKM,
        //                            DiemDau = DiemDau,
        //                            DiemCuoi = DiemCuoi,
        //                            DiemLayRong = DiemLayRong,
        //                            GhiChu = GhiChu,
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //        string DuplicateCus = "";

        //        foreach (var item in list)
        //        {
        //            var checkExists = await _context.CungDuong.Where(x => x.MaCungDuong == item.MaCungDuong).FirstOrDefaultAsync();
        //            ErrorInsert += 1;
        //            if (checkExists == null)
        //            {
        //                var addRoad = await _context.AddAsync(new GetRoadRequest()
        //                {
        //                    MaCungDuong = item.MaCungDuong.ToUpper(),
        //                    TenCungDuong = item.TenCungDuong,
        //                    Km = item.Km,
        //                    DiemDau = item.DiemDau,
        //                    DiemCuoi = item.DiemCuoi,
        //                    DiemLayRong = item.DiemLayRong,
        //                    GhiChu = item.GhiChu,
        //                });

        //                await _context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                DuplicateCus += item.MaCungDuong + ", ";
        //            }
        //        }

        //        if (ErrorValidate != "")
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
        //        }

        //        if (DuplicateCus.Length > 1)
        //        {
        //            return new BoolActionResult { Message = "Những khách hàng có mã " + DuplicateCus + " đã tồn tại", isSuccess = true };
        //        }
        //        else
        //        {
        //            return new BoolActionResult { Message = "OK", isSuccess = true };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BoolActionResult { Message = ex.ToString(), isSuccess = false, DataReturn = "Bị lỗi tại dòng: " + ErrorRow + " >>>Lỗi Insert dòng: " + ErrorInsert };
        //    }
        //}

        private async Task<string> ValiateRoad(string TenCungDuong, double SoKM, int DiemDau, int DiemCuoi, string GhiChu, string Action, string ErrorRow = "")
        {
            string ErrorValidate = "";

            if (DiemDau == DiemCuoi)
            {
                ErrorValidate += "Điểm đầu không được giống với điểm cuối \r\n";
            }

            if (TenCungDuong.Length > 50 || TenCungDuong.Length == 0)
            {
                ErrorValidate += "Tên cung đường không được rỗng hoặc nhiều hơn 50 ký tự \r\n";
            }

            //if (!Regex.IsMatch(TenCungDuong, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            //{
            //    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tên cung đường không được chứa ký tự đặc biệt \r\n";
            //}

            if (SoKM < 1)
            {
                ErrorValidate += "Số KM không được nhỏ hơn 1 \r\n";
            }

            if (Action == "Create")
            {
                var checkExists = await _context.CungDuong.Where(x => x.DiemDau == DiemDau && x.DiemCuoi == DiemCuoi).FirstOrDefaultAsync();
                if (checkExists != null)
                {
                    ErrorValidate += "Cung Đường đã tồn tại, " + checkExists.MaCungDuong + " - " + checkExists.TenCungDuong + " \r\n";
                }

                var checkDC = await _context.DiaDiem.Where(x => x.MaDiaDiem == DiemCuoi).FirstOrDefaultAsync();
                var checkDD = await _context.DiaDiem.Where(x => x.MaDiaDiem == DiemDau).FirstOrDefaultAsync();

                if (checkDD == null)
                {
                    ErrorValidate += "Mã điểm đầu không đúng, vui lòng xem lại \r\n";
                }

                if (checkDC == null)
                {
                    ErrorValidate += "Mã điểm cuối không đúng, vui lòng xem lại \r\n";
                }
            }
            return ErrorValidate;
        }

        public async Task<List<GetRoadRequest>> getListRoadOptionSelect(string MaKH, string ContractId)
        {
            if (string.IsNullOrEmpty(MaKH) && string.IsNullOrEmpty(ContractId))
            {
                var listRoad = await _context.CungDuong.Select(x => new GetRoadRequest()
                {
                    MaCungDuong = x.MaCungDuong,
                    TenCungDuong = x.TenCungDuong,
                }).ToListAsync();

                return listRoad;
            }

            var getList = from cd in _context.CungDuong
                          join bg in _context.BangGia
                          on cd.MaCungDuong equals bg.MaCungDuong
                          join
                          hd in _context.HopDongVaPhuLuc
                          on bg.MaHopDong equals hd.MaHopDong
                          orderby cd.MaCungDuong descending
                          select new { cd, bg, hd };

            if (!string.IsNullOrEmpty(MaKH))
            {
                getList = getList.Where(x => x.hd.MaKh == MaKH);
            }

            if (!string.IsNullOrEmpty(ContractId))
            {
                getList = getList.Where(x => x.hd.MaHopDong == ContractId);
            }

            var list = await getList.GroupBy(x => new { x.cd.MaCungDuong, x.cd.TenCungDuong }).Select(x => new GetRoadRequest()
            {
                MaCungDuong = x.Key.MaCungDuong,
                TenCungDuong = x.Key.TenCungDuong,
            }).ToListAsync();

            return list;
        }

        public async Task<List<GetRoadRequest>> getListRoadByPoint(int diemDau, int diemCuoi)
        {
            var listRoad = from cd in _context.CungDuong
                           where cd.DiemDau == diemDau
                           && cd.DiemCuoi == diemCuoi
                           select cd;

            return await listRoad.Select(x => new GetRoadRequest()
            {
                MaCungDuong = x.MaCungDuong,
                TenCungDuong = x.TenCungDuong,
            }).ToListAsync();
        }
    }
}