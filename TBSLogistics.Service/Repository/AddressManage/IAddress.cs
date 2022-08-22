using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.AddressManage
{
    public interface IAddress
    {
        Task<BoolActionResult> CreateAddress(CreateAddressRequest request);

        Task<BoolActionResult> EditAddress(int id, UpdateAddressRequest request);

        Task<List<TinhThanh>> GetProvinces();

        Task<List<QuanHuyen>> GetDistricts(int IdProvince);

        Task<List<XaPhuong>> GetWards(int IdDistricts);

        Task<PagedResponseCustom<GetAddressModel>> GetListAddress(PaginationFilter filter);

        Task<DiaDiem> GetAddressById(int IdAddress);

        Task<string> GetFullAddress(string address, int provinceId, int districtId, int wardId);

        Task<BoolActionResult> CreateProvince(int matinh,string tentinh,string phanloai);
        Task<BoolActionResult> CreateDistricts(int mahuyen,string tenhuyen,string phanloai,int parentcode);
        Task<BoolActionResult> CreateWard(List<WardModel> request);
    }
}
