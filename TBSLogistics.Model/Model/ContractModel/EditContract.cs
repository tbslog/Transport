using Microsoft.AspNetCore.Http;
using System;

namespace TBSLogistics.Model.Model.ContractModel
{
    public class EditContract
    {
        public string SoHopDongCha { get; set; }
        public int? NgayThanhToan { get; set; }
        public string TenHienThi { get; set; }
        public string MaPtvc { get; set; }
        public string PhanLoaiHopDong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; }
        public IFormFile FileContract { get; set; }
        public IFormFile FileCosting { get; set; }
        public int TrangThai { get; set; }
    }
}