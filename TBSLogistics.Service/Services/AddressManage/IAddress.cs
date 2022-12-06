using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Model.Model.TypeCommon;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.AddressManage
{
    public interface IAddress
    {
        Task<BoolActionResult> CreateAddress(CreateAddressRequest request);

        Task<BoolActionResult> EditAddress(int id, UpdateAddressRequest request);

        Task<List<TinhThanh>> GetProvinces();

        Task<List<QuanHuyen>> GetDistricts(int IdProvince);

        Task<List<XaPhuong>> GetWards(int IdDistricts);

        Task<List<ListTypeAddress>> GetListTypeAddress();

        Task<PagedResponseCustom<GetAddressModel>> GetListAddress(PaginationFilter filter);

        Task<GetAddressModel> GetAddressById(int IdAddress);

        Task<List<GetListAddress>> GetListAddress(string pointType);


        Task<string> GetFullAddress(string address, int provinceId, int districtId, int wardId);

        Task<BoolActionResult> ReadExcelFile(IFormFile formFile, CancellationToken cancellationToken);

        Task<BoolActionResult> CreateProvince(int matinh, string tentinh, string phanloai);
        Task<BoolActionResult> CreateDistricts(int mahuyen, string tenhuyen, string phanloai, int parentcode);

    }
}
