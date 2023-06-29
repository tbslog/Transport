using System;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.Bill
{
	public interface IBill
	{
		Task<GetBill> GetBillByCustomerId(string customerId, DateTime datePay, DateTime dateTime, string bank);

		Task<GetBill> GetBillByTransportId(string transportId, long? handlingId, DateTime dateTime, string bank);

		Task<PagedResponseCustom<ListBillHandling>> GetListBillHandling(PaginationFilter filter);

		Task<PagedResponseCustom<ListBillTransportWeb>> GetListBillCustomer(PaginationFilter filter);

		Task<PagedResponseCustom<ListBillHandling>> GetListHandlingToPick(PaginationFilter filter);

		Task<BoolActionResult> StoreDataHandlingToBill(StoreDataHandling request);

		Task<BoolActionResult> BlockDataBillByKy(string customerId, DateTime datePay, DateTime dateTime, string bank);
	}
}