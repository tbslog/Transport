using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.SubfeeLoloModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.LoloSubfeeManager
{
	public interface ILoloSubfee
	{
		Task<BoolActionResult> CreateSubfeeLolo(List<CreateOrUpdateSubfeeLolo> request, bool createByExcel);
		Task<PagedResponseCustom<ListSubfeeLolo>> GetListSubfeeLolo(PaginationFilter filter);
		Task<BoolActionResult> UpdateSubfeeLolo(int id, CreateOrUpdateSubfeeLolo request);
		Task<GetSubfeeLoloById> GetSubfeeLoloById(int id);
		Task<List<ListSubfeeLolo>> GetListSubfeeLoloExportExcel(PaginationFilter filter);
		Task<BoolActionResult> CreateSubfeeLoloByExcel(IFormFile formFile, CancellationToken cancellationToken);
		Task<BoolActionResult> ApproveSubfeeLolo(ApprovePriceTable request);
		Task<BoolActionResult> AutoAddSubfeeLolo(long handlingId);
	}
}
