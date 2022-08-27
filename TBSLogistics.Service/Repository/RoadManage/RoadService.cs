using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.RoadManage
{
    public class RoadService : IRoad
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;


        public RoadService(ICommon common, TMSContext context)
        {
            _common = common;
            _context = context;
        }

        public async Task<GetRoadRequest> GetRoadById(string MaCungDuong)
        {
            try
            {
                var getById = await _context.CungDuongs.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

                if (getById != null)
                {
                    return new GetRoadRequest()
                    {
                        MaCungDuong = getById.MaCungDuong,
                        TenCungDuong = getById.TenCungDuong,
                        MaHopDong = getById.MaHopDong,
                        Km = getById.Km,
                        DiemDau = getById.DiemDau,
                        DiemCuoi = getById.DiemCuoi,
                        DiemLayRong = getById.DiemLayRong,
                        GhiChu = getById.GhiChu,
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
                var checkExists = await _context.CungDuongs.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã cung đường đã tồn tại" };
                }

                await _context.AddAsync(new CungDuong()
                {
                    MaCungDuong = request.MaCungDuong,
                    TenCungDuong = request.TenCungDuong,
                    MaHopDong = request.MaHopDong,
                    Km = request.Km,
                    DiemDau = request.DiemDau,
                    DiemCuoi = request.DiemCuoi,
                    DiemLayRong = request.DiemLayRong,
                    GhiChu = request.GhiChu,
                    UpdateTime = DateTime.Now,
                    Createdtime = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await _common.Log("RoadManage", "UserId:" + TempData.UserID + " create new Road with Id: " + request.MaCungDuong);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới cung đường thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới cung đường thất bại" };
                }

            }
            catch (Exception ex)
            {
                await _common.Log("RoadManage", "UserId:" + TempData.UserID + " create new Road with Error: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UpdateRoad(string MaCungDuong, UpdateRoadRequest request)
        {
            try
            {
                var checkExists = await _context.CungDuongs.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

                if (checkExists == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã cung đường không tồn tại" };
                }

                checkExists.TenCungDuong = request.TenCungDuong;
                checkExists.Km = request.Km;
                checkExists.DiemDau = request.DiemDau;
                checkExists.DiemCuoi = request.DiemCuoi;
                checkExists.DiemLayRong = request.DiemLayRong;
                checkExists.GhiChu = request.GhiChu;
                checkExists.UpdateTime = DateTime.Now;

                _context.CungDuongs.Update(checkExists);


                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await _common.Log("RoadManage", "UserId:" + TempData.UserID + " Update Road with Id: " + MaCungDuong);
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

                var getData = from cungduong in _context.CungDuongs
                              select new { cungduong };

                var getAddress = from diadiem in _context.DiaDiems
                                 join phanloaidd in _context.LoaiDiaDiems
                                 on diadiem.MaLoaiDiaDiem equals phanloaidd.MaLoaiDiaDiem
                                 select new { diadiem, phanloaidd };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getData = getData.Where(x => x.cungduong.MaCungDuong.ToLower().Contains(filter.Keyword.ToLower()));
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    getData = getData.Where(x => x.cungduong.Createdtime.Date >= filter.fromDate && x.cungduong.Createdtime <= filter.toDate);
                }

                var totalRecords = await getData.CountAsync();

                var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListRoadRequest()
                {
                    MaCungDuong = x.cungduong.MaCungDuong,
                    TenCungDuong = x.cungduong.TenCungDuong,
                    MaHopDong = x.cungduong.MaHopDong,
                    Km = x.cungduong.Km,
                    DiemDau = getAddress.Where(y => y.diadiem.MaDiaDiem == x.cungduong.DiemDau).Select(y => y.diadiem.TenDiaDiem).FirstOrDefault(),
                    DiemCuoi = getAddress.Where(y => y.diadiem.MaDiaDiem == x.cungduong.DiemCuoi).Select(y => y.diadiem.TenDiaDiem).FirstOrDefault(),
                    DiemLayRong = getAddress.Where(y => y.diadiem.MaDiaDiem == x.cungduong.DiemLayRong).Select(y => y.diadiem.TenDiaDiem).FirstOrDefault(),
                    GhiChu = x.cungduong.GhiChu,
                    PhanLoaiDiaDiem = getAddress.Select(x => x.phanloaidd.TenPhanLoaiDiaDiem).FirstOrDefault()
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

        public async Task<BoolActionResult> ImportExcel(IFormFile formFile, CancellationToken cancellationToken)
        {
            int ErrorRow = 0;
            int ErrorInsert = 1;

            string ErrorValidate = "";

            try
            {
                if (formFile == null || formFile.Length <= 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
                }

                if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
                }

                var list = new List<CreateRoadRequest>();

                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream, cancellationToken);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        if (rowCount == 0)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "This file is empty" };
                        }

                        if (worksheet.Cells[1, 1].Value.ToString().Trim() != "Mã cung đường" ||
                            worksheet.Cells[1, 2].Value.ToString().Trim() != "Tên cung đường" ||
                            worksheet.Cells[1, 3].Value.ToString().Trim() != "Mã hợp đồng" ||
                            worksheet.Cells[1, 4].Value.ToString().Trim() != "Số KM" ||
                            worksheet.Cells[1, 5].Value.ToString().Trim() != "Điểm đầu" ||
                            worksheet.Cells[1, 6].Value.ToString().Trim() != "Điểm cuối" ||
                            worksheet.Cells[1, 7].Value.ToString().Trim() != "Điểm lấy rỗng" ||
                            worksheet.Cells[1, 8].Value.ToString().Trim() != "Ghi Chú"
                            )
                        {
                            return new BoolActionResult { isSuccess = false, Message = "File excel không đúng " };
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            ErrorRow = row;

                            string MaCungDuong = worksheet.Cells[row, 1].Value.ToString().Trim().ToUpper();
                            string TenCungDuong = worksheet.Cells[row, 2].Value.ToString().Trim();
                            string MaHopDong = worksheet.Cells[row, 3].Value.ToString().Trim();
                            double SoKM = double.Parse(worksheet.Cells[row, 4].Value.ToString().Trim());
                            int DiemDau = int.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
                            int DiemCuoi = int.Parse(worksheet.Cells[row, 6].Value.ToString().Trim());
                            int DiemLayRong = int.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
                            string GhiChu = worksheet.Cells[row, 8].Value.ToString().Trim();



                            ErrorValidate = await ValiateCustommer(MaCungDuong, TenCungDuong, MaHopDong, SoKM, DiemDau, DiemCuoi, DiemLayRong, GhiChu, ErrorRow.ToString());

                            if (ErrorValidate == "")
                            {
                                list.Add(new CreateRoadRequest
                                {
                                    MaCungDuong = MaCungDuong,
                                    TenCungDuong = TenCungDuong,
                                    MaHopDong = MaHopDong,
                                    Km = SoKM,
                                    DiemDau = DiemDau,
                                    DiemCuoi = DiemCuoi,
                                    DiemLayRong = DiemLayRong,
                                    GhiChu = GhiChu,
                                });
                            }
                        }
                    }
                }

                string DuplicateCus = "";

                foreach (var item in list)
                {
                    var checkExists = await _context.CungDuongs.Where(x => x.MaCungDuong == item.MaCungDuong).FirstOrDefaultAsync();
                    ErrorInsert += 1;
                    if (checkExists == null)
                    {
                        var addRoad = await _context.AddAsync(new GetRoadRequest()
                        {
                            MaCungDuong = item.MaCungDuong.ToUpper(),
                            TenCungDuong = item.TenCungDuong,
                            MaHopDong = item.MaHopDong,
                            Km = item.Km,
                            DiemDau = item.DiemDau,
                            DiemCuoi = item.DiemCuoi,
                            DiemLayRong = item.DiemLayRong,
                            GhiChu = item.GhiChu,
                        });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        DuplicateCus += item.MaCungDuong + ", ";
                    }
                }

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                if (DuplicateCus.Length > 1)
                {
                    return new BoolActionResult { Message = "Những khách hàng có mã " + DuplicateCus + " đã tồn tại", isSuccess = true };
                }
                else
                {
                    return new BoolActionResult { Message = "OK", isSuccess = true };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { Message = ex.ToString(), isSuccess = false, DataReturn = "Bị lỗi tại dòng: " + ErrorRow + " >>>Lỗi Insert dòng: " + ErrorInsert };
            }
        }

        private async Task<string> ValiateCustommer(string MaCungDuong, string TenCungDuong, string MaHopDong, double SoKM, int DiemDau, int DiemCuoi, int DiemLayRong, string GhiChu, string ErrorRow = "")
        {
            string ErrorValidate = "";

            if (MaCungDuong.Length != 10)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã cung đường phải dài 10 ký tự \r\n" ;
            }
            if (!Regex.IsMatch(MaCungDuong, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã cung đường không được chứa ký tự đặc biệt \r\n" ;
            }

            if (TenCungDuong.Length > 50 || TenCungDuong.Length == 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tên cung đường không được rỗng hoặc nhiều hơn 50 ký tự \r\n" ;
            }

            if (!Regex.IsMatch(TenCungDuong, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tên cung đường không được chứa ký tự đặc biệt \r\n" ;
            }

            if (MaHopDong.Length != 10)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã hợp đồng phải dài 10 ký tự \r\n" ;
            }
            if (!Regex.IsMatch(MaHopDong, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã hợp đồng không được chứa ký tự đặc biệt \r\n" ;
            }

            if(SoKM < 1)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Số KM không được nhỏ hơn 1 \r\n" ;
            }

            var checkAddress = await _context.DiaDiems.Where(x => x.MaDiaDiem == DiemCuoi || x.MaDiaDiem == DiemDau || x.MaDiaDiem == DiemLayRong).ToListAsync();

            if(checkAddress.Count != 3)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã địa điểm không đúng, vui lòng xem lại \r\n" ;
            }

            return  ErrorValidate;
        }

    }
}
