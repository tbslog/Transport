using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.BillOfLadingManage
{
    public interface IBillOfLading
    {
        Task<LoadDataHandling> LoadDataHandling();
        Task<BoolActionResult> CreateTransport(CreateTransport request);
        Task<BoolActionResult> UpdateTransport(string transPortId, UpdateTransport request);
        Task<GetTransport> GetTransportById(string transportId);
        Task<PagedResponseCustom<ListTransport>> GetListTransport(ListFilter listFilter, PaginationFilter filter);
        Task<GetHandling> GetHandlingById(int id);
        Task<BoolActionResult> UpdateHandling(int id, UpdateHandling request);
        Task<BoolActionResult> UploadFile(UploadImagesHandling request);
        Task<List<Attachment>> GetListImageByHandlingId(int handlingId);
        Task<BoolActionResult> DeleteImageById(int imageId);
        Task<Attachment> GetImageById(int id);
        Task<PagedResponseCustom<ListHandling>> GetListHandling(string transportId, ListFilter listFilter, PaginationFilter filter);
        Task<PagedResponseCustom<ListHandling>> GetListHandlingLess(ListFilter listFilter, PaginationFilter filter);
        Task<BoolActionResult> CreateTransportLess(CreateTransportLess request);
        Task<LoadJoinTransports> LoadJoinTransport(JoinTransports request);
        Task<BoolActionResult> CreateHandlingLess(CreateHandlingLess request);
        Task<BoolActionResult> UpdateTransportLess(string transportId, UpdateTransportLess request);
        Task<BoolActionResult> UpdateHandlingLess(string handlingId, UpdateHandlingLess request);
        Task<UpdateTransportLess> GetTransportLessById(string transportId);
        Task<PagedResponseCustom<ListHandling>> GetListHandlingByTransportId(string transportId, PaginationFilter filter);
        Task<BoolActionResult> AcceptOrRejectTransport(string transportId, int action);
        Task<string> LayTrongTaiXe(string vehicleType, string DonVi, double giaTri);
        Task<BoolActionResult> ChangeImageName(int id, string newName);
        Task<BoolActionResult> CreateTransportByExcel(IFormFile formFile, CancellationToken cancellationToken);
        Task<BoolActionResult> ChangeStatusHandling(int id, string maChuyen);
        Task<BoolActionResult> CancelHandling(int id, string note = null);
        Task<BoolActionResult> CancelHandlingByCustomer(int id, string note = null);
    }
}
