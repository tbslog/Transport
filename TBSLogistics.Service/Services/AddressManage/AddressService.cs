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
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.Model.TypeCommon;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.AddressManage
{
    public class AddressService : IAddress
    {
        private readonly TMSContext _context;
        private readonly ICommon _common;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public AddressService(TMSContext vanChuyenContext, ICommon common, IHttpContextAccessor httpContextAccessor)
        {
            _common = common;
            _context = vanChuyenContext;
            _httpContextAccessor = httpContextAccessor;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }

        public async Task<BoolActionResult> CreateDistricts(int mahuyen, string tenhuyen, string phanloai, int parentcode)
        {
            var add = await _context.AddAsync(new QuanHuyen()
            {
                MaHuyen = mahuyen,
                TenHuyen = tenhuyen,
                PhanLoai = phanloai,
                ParentCode = parentcode
            });

            await _context.SaveChangesAsync();

            return new BoolActionResult { isSuccess = true, Message = "OK" };
        }

        public async Task<BoolActionResult> CreateProvince(int matinh, string tentinh, string phanloai)
        {
            var add = await _context.AddAsync(new TinhThanh()
            {
                MaTinh = matinh,
                TenTinh = tentinh,
                PhanLoai = phanloai
            });

            await _context.SaveChangesAsync();

            return new BoolActionResult { isSuccess = true, Message = "OK" };
        }

        public async Task<BoolActionResult> CreateAddress(CreateAddressRequest request)
        {
            try
            {
                var checkExists = await _context.DiaDiem.Where(x => x.TenDiaDiem == request.TenDiaDiem).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Địa điểm này đã tồn tại" };
                }

                //string FullAddress = await GetFullAddress(request.SoNha, request.MaTinh, request.MaHuyen, request.MaPhuong);
                string ErrorValidate = await ValiateAddress(request.DiaDiemCha, request.TenDiaDiem, request.SoNha, request.MaGps, request.PhanLoaiLoaiDiaDiem);

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                await _context.AddAsync(new DiaDiem()
                {
                    LoaiDiaDiem = request.DiaDiemCha == null ? "KhuVuc" : "Diem",
                    DiaDiemCha = request.DiaDiemCha,
                    TenDiaDiem = request.TenDiaDiem.Trim(),
                    MaQuocGia = null,
                    MaTinh = request.MaTinh,
                    MaHuyen = request.MaHuyen,
                    MaPhuong = request.MaPhuong,
                    SoNha = request.SoNha,
                    DiaChiDayDu = "",
                    MaGps = request.MaGps.Trim(),
                    NhomDiaDiem = request.PhanLoaiLoaiDiaDiem,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    Creator = tempData.UserName,
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("AddressManage", "UserId: " + tempData.UserName + " create new Address with Data: " + JsonSerializer.Serialize(request));
					await _common.LogTimeUsedOfUser(tempData.Token);
					return new BoolActionResult { isSuccess = true, Message = "Tạo mới địa điểm thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới địa điểm thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("AddressManage", "UserId: " + tempData.UserName + " create new Address with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> EditAddress(int id, UpdateAddressRequest request)
        {
            try
            {
                var getAddress = await _context.DiaDiem.Where(x => x.MaDiaDiem == id).FirstOrDefaultAsync();

                if (getAddress == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Địa điểm không tồn tại" };
                }

                //string FullAddress = await GetFullAddress(request.SoNha, request.MaTinh, request.MaHuyen, request.MaPhuong);

                string ErrorValidate = await ValiateAddress(request.DiaDiemCha, request.TenDiaDiem, request.SoNha, request.MaGps, request.NhomDiaDiem);

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                if (getAddress.DiaDiemCha == null)
                {
                    var checkHasChild = await _context.DiaDiem.Where(x => x.DiaDiemCha == id).ToListAsync();
                    if (checkHasChild.Count > 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Khu Vực này đã chứa nhiều điểm, không thể điều chỉnh thành điểm" };
                    }
                }

                getAddress.LoaiDiaDiem = request.DiaDiemCha == null ? "KhuVuc" : "Diem";
                getAddress.DiaDiemCha = request.DiaDiemCha;
                getAddress.TenDiaDiem = request.TenDiaDiem.Trim();
                getAddress.MaTinh = request.MaTinh;
                getAddress.MaHuyen = request.MaHuyen;
                getAddress.MaPhuong = request.MaPhuong;
                getAddress.SoNha = request.SoNha;
                getAddress.MaGps = request.MaGps;
                getAddress.NhomDiaDiem = request.NhomDiaDiem;
                getAddress.UpdatedTime = DateTime.Now;
                getAddress.Updater = tempData.UserName;

                //if (getAddress.DiaChiDayDu != request.DiaChiDayDu)
                //{
                //    getAddress.DiaChiDayDu = FullAddress;
                //}

                _context.Update(getAddress);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("AddressManage", "UserId: " + tempData.UserName + " edit Address with data: " + JsonSerializer.Serialize(request));
					await _common.LogTimeUsedOfUser(tempData.Token);
					return new BoolActionResult { isSuccess = true, Message = "Cập nhật địa điểm thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật địa điểm thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("AddressManage", "UserId: " + tempData.UserName + " edit Address with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<GetAddressModel> GetAddressById(int IdAddress)
        {
            var getAddress = await _context.DiaDiem.Where(x => x.MaDiaDiem == IdAddress).FirstOrDefaultAsync();

            return new GetAddressModel()
            {
                DiaDiemCha = getAddress.DiaDiemCha,
                MaDiaDiem = getAddress.MaDiaDiem,
                TenDiaDiem = getAddress.TenDiaDiem,
                LoaiDiaDiem = getAddress.LoaiDiaDiem,
                PhanLoaiDiaDiem = getAddress.NhomDiaDiem,
                MaHuyen = getAddress.MaHuyen,
                MaPhuong = getAddress.MaPhuong,
                MaTinh = getAddress.MaTinh,
                SoNha = getAddress.SoNha,
                MaGps = getAddress.MaGps,
            };
        }

        public async Task<GetListPlaceSelect> GetListPlaceSelect()
        {
            var getListPlace = await _context.DiaDiem.Select(x => new PlaceSelect()
            {
                MaDiaDiem = x.MaDiaDiem,
                TenDiaDiem = x.TenDiaDiem
            }).ToListAsync();

            return new GetListPlaceSelect()
            {
                listGetEmptyPlace = getListPlace,
                listFirstPlace = getListPlace,
                listSecondPlace = getListPlace
            };
        }

        public async Task<List<QuanHuyen>> GetDistricts(int IdProvince)
        {
            var ListDistricts = await _context.QuanHuyen.Where(x => x.ParentCode == IdProvince).ToListAsync();
            return ListDistricts;
        }

        public async Task<string> GetFullAddress(string address, int provinceId, int districtId, int wardId)
        {
            try
            {
                var getProvinceName = await _context.TinhThanh.Where(x => x.MaTinh == provinceId).Select(x => new { x.TenTinh, x.MaTinh }).FirstOrDefaultAsync();
                var getDistrictName = await _context.QuanHuyen.Where(x => x.MaHuyen == districtId && x.ParentCode == getProvinceName.MaTinh).Select(x => new { x.TenHuyen, x.MaHuyen }).FirstOrDefaultAsync();
                var getWardName = await _context.XaPhuong.Where(x => x.MaPhuong == wardId && x.ParentCode == getDistrictName.MaHuyen).Select(x => x.TenPhuong).FirstOrDefaultAsync();

                if (getProvinceName == null || getDistrictName == null || getWardName == null)
                {
                    return "";
                }

                var fullAddress = address + ", " + getWardName + ", " + getDistrictName.TenHuyen + ", " + getProvinceName.TenTinh;

                return fullAddress;
            }
            catch (Exception ex)
            {
                return "";
                throw;
            }
        }

        public async Task<PagedResponseCustom<GetAddressModel>> GetListAddress(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var getData = from ar in _context.DiaDiem
                              join loaiDiaDiem in _context.LoaiDiaDiem
                              on ar.NhomDiaDiem equals loaiDiaDiem.MaLoaiDiaDiem
                              orderby ar.CreatedTime descending
                              select new { ar, loaiDiaDiem };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getData = getData.Where(x => x.ar.TenDiaDiem.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    getData = getData.Where(x => x.ar.CreatedTime.Date >= filter.fromDate && x.ar.CreatedTime <= filter.toDate);
                }

                var totalRecords = await getData.CountAsync();

                var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new GetAddressModel()
                {
                    MaDiaDiem = x.ar.MaDiaDiem,
                    KhuVuc = _context.DiaDiem.Where(y => y.MaDiaDiem == x.ar.DiaDiemCha).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    PhanLoaiDiaDiem = x.loaiDiaDiem.TenPhanLoaiDiaDiem,
                    TenDiaDiem = x.ar.TenDiaDiem,
                    DiaChiDayDu = x.ar.DiaChiDayDu,
                    MaGps = x.ar.MaGps,
                    LoaiDiaDiem = x.ar.LoaiDiaDiem,
                    CreatedTime = x.ar.CreatedTime,
                    UpdatedTime = x.ar.UpdatedTime,
                }).ToListAsync();

                return new PagedResponseCustom<GetAddressModel>()
                {
                    paginationFilter = validFilter,
                    totalCount = totalRecords,
                    dataResponse = pagedData
                };
            }
            catch (Exception ex)
            {
                return new PagedResponseCustom<GetAddressModel>();
            }
        }

        public async Task<List<TinhThanh>> GetProvinces()
        {
            var ListProvinces = await _context.TinhThanh.ToListAsync();
            return ListProvinces;
        }

        public async Task<List<XaPhuong>> GetWards(int IdDistricts)
        {
            var ListWards = await _context.XaPhuong.Where(x => x.ParentCode == IdDistricts).ToListAsync();
            return ListWards;
        }

        public async Task<List<ListTypeAddress>> GetListTypeAddress()
        {
            var list = await _context.LoaiDiaDiem.ToListAsync();

            return list.Select(x => new ListTypeAddress()
            {
                MaLoaiDiaDiem = x.MaLoaiDiaDiem,
                TenLoaiDiaDiem = x.TenPhanLoaiDiaDiem
            }).ToList();
        }

        //public async Task<BoolActionResult> ReadExcelFile(IFormFile formFile, CancellationToken cancellationToken)
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

        //        var list = new List<CreateAddressRequest>();

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

        //                if (worksheet.Cells[1, 1].Value.ToString().Trim() != "Tên Địa Điểm" ||
        //                    worksheet.Cells[1, 2].Value.ToString().Trim() != "Mã GPS" ||
        //                    worksheet.Cells[1, 3].Value.ToString().Trim() != "Số Nhà" ||
        //                    worksheet.Cells[1, 4].Value.ToString().Trim() != "Mã Loại Địa Điểm" ||
        //                    worksheet.Cells[1, 5].Value.ToString().Trim() != "Mã Tỉnh" ||
        //                    worksheet.Cells[1, 6].Value.ToString().Trim() != "Mã Huyện" ||
        //                    worksheet.Cells[1, 7].Value.ToString().Trim() != "Mã Phường"
        //                    )
        //                {
        //                    return new BoolActionResult { isSuccess = false, Message = "File excel không đúng " };
        //                }

        //                for (int row = 2; row <= rowCount; row++)
        //                {
        //                    ErrorRow = row;

        //                    string TenDiaDiem = worksheet.Cells[row, 1].Value.ToString().Trim();
        //                    string SoNha = worksheet.Cells[row, 3].Value.ToString().Trim();
        //                    int MaTinh = int.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
        //                    int MaHuyen = int.Parse(worksheet.Cells[row, 6].Value.ToString().Trim());
        //                    int MaPhuong = int.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
        //                    string MaGPS = worksheet.Cells[row, 2].Value.ToString().Trim();
        //                    string MaLoaiDiaDiem = worksheet.Cells[row, 4].Value.ToString().Trim();

        //                    var FullAddress = await GetFullAddress(SoNha, MaTinh, MaHuyen, MaPhuong);

        //                    ErrorValidate = await ValiateAddress(TenDiaDiem, SoNha, MaGPS, FullAddress, MaLoaiDiaDiem, ErrorRow.ToString());

        //                    if (ErrorValidate == "")
        //                    {
        //                        list.Add(new CreateAddressRequest
        //                        {
        //                            TenDiaDiem = TenDiaDiem,
        //                            MaQuocGia = 1,
        //                            SoNha = SoNha,
        //                            MaTinh = MaTinh,
        //                            MaHuyen = MaHuyen,
        //                            MaPhuong = MaPhuong,
        //                            DiaChiDayDu = FullAddress,
        //                            MaGps = MaGPS,
        //                            MaLoaiDiaDiem = MaLoaiDiaDiem,
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //        string DuplicateCus = "";

        //        foreach (var item in list)
        //        {
        //            var checkExists = await _context.DiaDiem.Where(x => x.TenDiaDiem.ToLower() == item.TenDiaDiem.ToLower()).FirstOrDefaultAsync();
        //            ErrorInsert += 1;
        //            if (checkExists == null)
        //            {
        //                var addAddress = await _context.AddAsync(new DiaDiem()
        //                {
        //                    TenDiaDiem = item.TenDiaDiem,
        //                    MaQuocGia = item.MaQuocGia,
        //                    MaTinh = item.MaTinh,
        //                    MaHuyen = item.MaHuyen,
        //                    MaPhuong = item.MaPhuong,
        //                    SoNha = item.SoNha,
        //                    DiaChiDayDu = item.DiaChiDayDu,
        //                    MaGps = item.MaGps,
        //                    MaLoaiDiaDiem = item.MaLoaiDiaDiem,
        //                    CreatedTime = DateTime.Now,
        //                    UpdatedTime = DateTime.Now
        //                });

        //                await _context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                DuplicateCus += item.TenDiaDiem + ", ";
        //            }
        //        }

        //        if (ErrorValidate != "")
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
        //        }

        //        if (DuplicateCus.Length > 1)
        //        {
        //            return new BoolActionResult { Message = "Những Địa điểm có mã " + DuplicateCus + " đã tồn tại", isSuccess = true };
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

        private async Task<string> ValiateAddress(int? DiaDiemCha, string TenDiaDiem, string SoNha, string MaGps, string PhanLoaiDiaDiem, string ErrorRow = "")
        {
            string ErrorValidate = "";

            if (DiaDiemCha.HasValue)
            {
                var checkParent = await _context.DiaDiem.Where(x => x.MaDiaDiem == DiaDiemCha).FirstOrDefaultAsync();
                if (checkParent == null)
                {
                    ErrorValidate += "Lỗi dòng >>>" + ErrorRow + " - Mã loại địa điểm không tồn tại \r\n";
                }
            }

            var CheckMaLoai = await _context.LoaiDiaDiem.Where(x => x.MaLoaiDiaDiem == PhanLoaiDiaDiem).FirstOrDefaultAsync();

            if (CheckMaLoai == null)
            {
                ErrorValidate += "Lỗi dòng >>>" + ErrorRow + " - Mã loại địa điểm không tồn tại \r\n";
            }

            if (TenDiaDiem.Length > 50 || TenDiaDiem.Length == 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tên địa điểm không được rỗng hoặc nhiều hơn 50 ký tự \r\n";
            }

            if (SoNha.Length > 100)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Số nhà không được nhiều hơn 100 ký tự \r\n";
            }

            //if (MaGps.Length == 0 || MaGps.Length > 50)
            //{
            //    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã GPS không được rỗng hoặc nhiều hơn 50 ký tự \r\n";
            //}

            //if (FullAddress == "")
            //{
            //    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã tỉnh, mã huyện, mã phường không khớp, vui lòng kiểm tra lại \r\n";
            //}

            return ErrorValidate;
        }

        public async Task<List<GetListAddress>> GetListAddressSelect(string pointType, string type)
        {
            try
            {
                var list = from dd in _context.DiaDiem
                           join ddt in _context.LoaiDiaDiem
                           on dd.NhomDiaDiem equals ddt.MaLoaiDiaDiem
                           select dd;

                if (!string.IsNullOrEmpty(pointType))
                {
                    list = list.Where(x => x.NhomDiaDiem == pointType);
                }

                if (!string.IsNullOrEmpty(type))
                {
                    list = list.Where(x => x.LoaiDiaDiem == type);
                }

                var data = await list.Select(x => new GetListAddress()
                {
                    MaDiaDiem = x.MaDiaDiem,
                    TenDiaDiem = x.TenDiaDiem,
                    LoaiDiaDiem = x.LoaiDiaDiem
                }).ToListAsync();

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}