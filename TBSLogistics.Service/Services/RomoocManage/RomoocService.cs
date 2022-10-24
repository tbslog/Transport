using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RomoocModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Services.RomoocManage
{
    public class RomoocService : IRomooc
    {
        private readonly ICommon _common;
        private readonly TMSContext _TMScontext;

        public RomoocService(ICommon common, TMSContext context)
        {
            _common = common;
            _TMScontext = context;
        }

        public async Task<BoolActionResult> CreateRomooc(CreateRomooc request)
        {
            try
            {
                var checkExists = await _TMScontext.Romooc.Where(x => x.MaRomooc == request.MaRomooc).FirstOrDefaultAsync();
                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã Romooc đã tồn tại" };
                }
                string ErrorValidate = await ValidateCreat(request.MaRomooc, request.MaLoaiRomooc);
                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }
                await _TMScontext.Romooc.AddAsync(new Romooc()
                {
                    MaRomooc = request.MaRomooc.ToUpper(),
                    KetCauSan = request.KetCauSan,
                    SoGuRomooc = request.SoGuRomooc,
                    ThongSoKyThuat = request.ThongSoKyThuat,
                    MaLoaiRomooc = request.MaLoaiRomooc,
                    TrangThai = 1,
                    UpdatedTime = DateTime.Now,
                    CreatedTime = DateTime.Now
                });
                var result = await _TMScontext.SaveChangesAsync();
                if (result > 0)
                {
                    await _common.Log("RomoocManage", "UserId: " + TempData.UserID + " create new Romooc with id: " + request.MaRomooc);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới Romooc thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới Romooc thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("RomoocManage", "UserId: " + TempData.UserID + " create new Romooc with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> EditRomooc(string MaRomooc, EditRomooc request)
        {
            try
            {
                var getRomooc = await _TMScontext.Romooc.Where(x => x.MaRomooc == MaRomooc).FirstOrDefaultAsync();
                if (getRomooc == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã Romooc không tồn tại" };
                }
            
                var IsMaLoaiRomooc = await _TMScontext.LoaiRomooc.Where(x => x.MaLoaiRomooc == request.MaLoaiRomooc).FirstOrDefaultAsync();
                if (IsMaLoaiRomooc == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã Loại Romooc không tồn tại" };
                }

                getRomooc.KetCauSan = request.KetCauSan;
                getRomooc.SoGuRomooc = request.SoGuRomooc;
                getRomooc.ThongSoKyThuat = request.ThongSoKyThuat;
                getRomooc.MaLoaiRomooc = request.MaLoaiRomooc;
                getRomooc.UpdatedTime = DateTime.Now;

                _TMScontext.Update(getRomooc);
                var result = await _TMScontext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("RomoocManage", "UserId: " + TempData.UserID + " update driver with id: " + MaRomooc);
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật Romooc thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật Romooc thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("RomoocManage", "UserId: " + TempData.UserID + " Edit Romooc with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = "Cập nhật Romooc thất bại" };
            }
        }

        public async Task<BoolActionResult> DeleteRomooc(string MaRomooc)
        {
            try
            {
                var getRomooc = await _TMScontext.Romooc.Where(x => x.MaRomooc == MaRomooc).FirstOrDefaultAsync();
                if (getRomooc == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã Romooc không tồn tại" };
                }
                var CheckTT = await _TMScontext.Romooc.Where(x => x.TrangThai == 1).FirstOrDefaultAsync();
                if (getRomooc == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Romooc phải ở trạng thái hoạt động" };
                }
                getRomooc.TrangThai = 2;
                _TMScontext.Update(getRomooc);
                var result = await _TMScontext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("RomoocManage", "UserId: " + TempData.UserID + " Delete driver with id: " + MaRomooc);
                    return new BoolActionResult { isSuccess = true, Message = "Xóa Romooc thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Xóa Romooc thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("RomoocManage", "UserId: " + TempData.UserID + " Delete Romooc with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = "Xóa Romooc thất bại" };
            }
        }

        public async Task<PagedResponseCustom<ListRomooc>> GetListRomooc(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var listData = from romooc in _TMScontext.Romooc
                               orderby romooc.CreatedTime descending
                               select new { romooc };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.romooc.MaRomooc.Contains(filter.Keyword));
                }
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.romooc.MaLoaiRomooc.Contains(filter.Keyword));
                }
                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    listData = listData.Where(x => x.romooc.CreatedTime.Date >= filter.fromDate && x.romooc.CreatedTime.Date <= filter.toDate);
                }
                var totalCount = await listData.CountAsync();

                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListRomooc()
                {
                    MaRomooc = x.romooc.MaRomooc,
                    KetCauSan = x.romooc.KetCauSan,
                    SoGuRomooc = x.romooc.SoGuRomooc,
                    ThongSoKyThuat = x.romooc.ThongSoKyThuat,
                    MaLoaiRomooc = x.romooc.MaLoaiRomooc,
                    TrangThai = x.romooc.TrangThai,
                    UpdateTime = x.romooc.UpdatedTime,
                    Createdtime = x.romooc.CreatedTime
                }).ToListAsync();
                return new PagedResponseCustom<ListRomooc>()
                {
                    dataResponse = pagedData,
                    totalCount = totalCount,
                    paginationFilter = validFilter
                };
            }
            catch (Exception ex)
            {
                return new PagedResponseCustom<ListRomooc>();
            }
        }

        public async Task<ListRomooc> GetRomoocById(string MaRomooc)
        {
            var listRomooc = await _TMScontext.Romooc.Where(x => x.MaRomooc == MaRomooc).Select(x => new ListRomooc()
            {
                MaRomooc = x.MaRomooc,
                KetCauSan = x.KetCauSan,
                SoGuRomooc = x.SoGuRomooc,
                ThongSoKyThuat = x.ThongSoKyThuat,
                MaLoaiRomooc = x.MaLoaiRomooc,
                TrangThai = x.TrangThai,
                UpdateTime = x.UpdatedTime,
                Createdtime = x.CreatedTime
            }).FirstOrDefaultAsync();
            return listRomooc;
        }

        public async Task<List<LoaiRomooc>> GetListSelectRomoocType()
        {
            var list = await _TMScontext.LoaiRomooc.ToListAsync();

            return list;
        }

        private async Task<string> ValidateCreat(string MaRomooc, string MaLoaiRomooc, string ErrorRow = "")
        {
            string ErrorValidate = "";

            if (!Regex.IsMatch(MaRomooc, "^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9]+(?<![_.])$"))
            {
                ErrorValidate += " - Mã Tài Xế khÔng được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }
            var checkTT = await _TMScontext.LoaiRomooc.Where(x => x.MaLoaiRomooc == MaLoaiRomooc).FirstOrDefaultAsync();
            if (checkTT == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Romooc: " + MaLoaiRomooc + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            return ErrorValidate;
        }
    }
}