using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.BillOfLadingManage
{
    public interface IBillOfLading
    {
        Task<LoadDataHandling> LoadDataHandling();
        Task<BoolActionResult> CreateHandling(CreateHandling request);
        Task<BoolActionResult> CreateTransport(CreateTransport request);
        Task<BoolActionResult> UpdateTransport(string transPortId, UpdateTransport request);
        Task<GetTransport> GetTransportById(string transportId);
        Task<PagedResponseCustom<ListTransport>> GetListTransport(string[] listCustomer,PaginationFilter filter);
        Task<GetHandling> GetHandlingById(int id);
        Task<BoolActionResult> UpdateHandling(int id, UpdateHandling request);
        Task<BoolActionResult> UploadFile(UploadImagesHandling request);
        Task<List<Attachment>> GetListImageByHandlingId(int handlingId);
        Task<BoolActionResult> DeleteImageById(int imageId);
        Task<BoolActionResult> SetRunning(int id);
        Task<BoolActionResult> CancelHandling(int id);
        Task<Attachment> GetImageById(int id);
        Task<PagedResponseCustom<ListHandling>> GetListHandling(string transportId, string[] customers, PaginationFilter filter);
        Task<PagedResponseCustom<ListHandling>> GetListHandlingLess(string[] customers, PaginationFilter filter);
        Task<ListPoint> LoadDataRoadTransportByCusId(string customerId);
        Task<BoolActionResult> CloneHandling(int id);
        Task<BoolActionResult> RemoveHandling(int id);
        Task<BoolActionResult> CreateTransportLess(CreateTransportLess request);
        Task<LoadJoinTransports> LoadJoinTransport(JoinTransports request);
        Task<BoolActionResult> CreateHandlingLess(CreateHandlingLess request);
        Task<BoolActionResult> UpdateTransportLess(string transportId, UpdateTransportLess request);
        Task<BoolActionResult> UpdateHandlingLess(string handlingId, UpdateHandlingLess request);
        Task<UpdateTransportLess> GetTransportLessById(string transportId);
        Task<BoolActionResult> SetRunningLess(string handlingId);
        Task<BoolActionResult> CancelHandlingLess(string handlingId);

    }
}
