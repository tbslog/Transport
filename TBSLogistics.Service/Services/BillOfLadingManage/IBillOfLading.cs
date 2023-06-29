using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.FileModel;
using TBSLogistics.Model.Model.MailSettings;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.Model.VehicleModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.BillOfLadingManage
{
    public interface IBillOfLading
    {
        Task<BoolActionResult> CreateTransport(CreateTransport request);
        Task<BoolActionResult> UpdateTransport(string transPortId, UpdateTransport request);
        Task<GetTransport> GetTransportById(string transportId);
        Task<PagedResponseCustom<ListTransport>> GetListTransport(ListFilter listFilter, PaginationFilter filter);
        Task<GetHandling> GetHandlingById(int id);
        Task<BoolActionResult> UpdateHandling(long id, UpdateHandling request);
        Task<List<DocumentType>> GetListImageByHandlingId(long handlingId);
        Task<BoolActionResult> DeleteImageById(int docId);
        Task<Attachment> GetImageById(long imageId);
        Task<PagedResponseCustom<ListHandling>> GetListHandlingLess(ListFilter listFilter, PaginationFilter filter);
        Task<BoolActionResult> CreateTransportLess(CreateTransportLess request);
        Task<LoadJoinTransports> LoadJoinTransport(JoinTransports request);
        Task<BoolActionResult> CreateHandlingLess(CreateHandlingLess request);
        Task<BoolActionResult> UpdateTransportLess(string transportId, UpdateTransportLess request);
        Task<BoolActionResult> UpdateHandlingLess(string handlingId, UpdateHandlingLess request);
        Task<UpdateTransportLess> GetTransportLessById(string transportId);
        Task<PagedResponseCustom<ListHandling>> GetListHandlingByTransportId(string transportId, PaginationFilter filter);
        Task<BoolActionResult> AcceptOrRejectTransport(string transportId, int action);
        Task<GetTonnageVehicle> LayTrongTaiXe(string vehicleType);
        Task<BoolActionResult> CreateTransportByExcel(IFormFile formFile, CancellationToken cancellationToken);
        Task<BoolActionResult> ChangeStatusHandling(int id, string maChuyen);
        Task<BoolActionResult> CancelHandling(int id, string note = null);
        Task<BoolActionResult> CancelHandlingByCustomer(int id, string note = null);
        Task<DocumentType> GetDocById(int docId);
        Task<BoolActionResult> UpdateDoc(int docId, UpdateDoc request);
        Task<BoolActionResult> CreateDoc(CreateDoc request);
        Task<BoolActionResult> SendMailToSuppliers(GetIdHandling handlingIds);
        Task<BoolActionResult> RestartHandling(long handlingId);
        Task<BoolActionResult> CancelTransport(string transportId);
        Task<BoolActionResult> ChangeSecondPlace(ChangeSecondPlaceOfHandling request);
        Task<BoolActionResult> SetSupplierForHandling(SetSupplierForHandling request);
        Task<BoolActionResult> FastCompleteTransport(List<long> ids);
	}
}
