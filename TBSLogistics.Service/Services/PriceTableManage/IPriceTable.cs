﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.PricelistManage
{
    public interface IPriceTable
    {
        Task<BoolActionResult> CreatePriceTable(List<CreatePriceListRequest> request, bool noContract, bool createByExcel = false);
		Task<PagedResponseCustom<ListCustomerOfPriceTable>> GetListPriceTable(PaginationFilter filter);
        Task<PagedResponseCustom<GetPriceListRequest>> GetListPriceTableByContractId(string contractId, string onlyContractId, ListFilter listFilter, PaginationFilter filter);
        Task<List<GetPriceListRequest>> GetListPriceTableByCustommerId(string MaKH);
        Task<PagedResponseCustom<ListApprove>> GetListPriceTableApprove(string contractId, PaginationFilter filter);
        Task<BoolActionResult> ApprovePriceTable(ApprovePriceTable request);
        Task<BoolActionResult> UpdatePriceTable(int id, GetPriceListById request);
        Task<GetPriceListById> GetPriceTableById(int id);
        Task<List<GetPriceListRequest>> GetListPriceTableExportExcel(string cusType);
        Task<BoolActionResult> CreatePriceByExcel(IFormFile formFile, bool noContract, CancellationToken cancellationToken);
      
        Task<BoolActionResult> RevertPriceTableOfHandling(string contractId, string cusId);
		Task<GetPriceListRequest> GetPriceTable(string MaKH, string accountId, int firstPlace, int secondPlace, int? emptyPlace, string MaDVT, string LoaiHangHoa, string LoaiPhuongTien, string MaPTVC);
	}
}
