﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.PricelistManage
{
    public interface IPriceTable
    {
        Task<BoolActionResult> CreatePriceTable(List<CreatePriceListRequest> request);

        Task<PagedResponseCustom<GetListPiceTableRequest>> GetListPriceTable(PaginationFilter filter);
        Task<PagedResponseCustom<GetPriceListRequest>> GetListPriceTableByContractId(string contractId, int PageNumber, int PageSize);
        Task<List<GetPriceListRequest>> GetListPriceTableByCustommerId(string MaKH);
        Task<PagedResponseCustom<ListApprove>> GetListPriceTableApprove(PaginationFilter filter);
        Task<BoolActionResult> ApprovePriceTable(int Id, int choose);

    }
}
