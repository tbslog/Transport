﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.BillOfLadingManage
{
    public interface IBillOfLading
    {
        Task<LoadDataHandling> LoadDataHandling(string RoadId);
        Task<BoolActionResult> CreateHandling(CreateHandling request);
        Task<BoolActionResult> CreateTransport(CreateTransport request);
        Task<BoolActionResult> UpdateTransport(string transPortId, UpdateTransport request);
        Task<GetTransport> GetTransportById(string transportId);
        Task<PagedResponseCustom<ListTransport>> GetListTransport(PaginationFilter filter);
        Task<List<ListHandling>> GetListHandlingByTransportId(string transPortId);
        Task<GetHandling> GetHandlingById(int id);
        Task<BoolActionResult> UpdateHandling(int id, UpdateHandling request);
        Task<BoolActionResult> UploadFile(UploadImagesHandling request);
        Task<List<Attachment>> GetListImageByHandlingId(int handlingId);
        Task<BoolActionResult> DeleteImageById(int imageId);
        Task<Attachment> GetImageById(int id);
    }
}