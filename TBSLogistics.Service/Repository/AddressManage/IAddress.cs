﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.AddressModel;

namespace TBSLogistics.Service.Repository.AddressManage
{
    public interface IAddress
    {
        Task<BoolActionResult> CreateAddress(CreateAddressRequest request);

        Task<BoolActionResult> EditAddress(int id, UpdateAddressRequest request);

        Task<List<TinhThanh>> GetProvinces();

        Task<List<QuanHuyen>> GetDistricts(int IdProvince);

        Task<List<XaPhuong>> GetWards(int IdDistricts);

        Task<List<DiaDiem>> GetListAddress();

        Task<DiaDiem> GetAddressById(int IdAddress);


        Task<BoolActionResult> CreateProvince(int matinh,string tentinh,string phanloai);
        Task<BoolActionResult> CreateDistricts(int mahuyen,string tenhuyen,string phanloai,int parentcode);
        Task<BoolActionResult> CreateWard(List<WardModel> request);
    }
}