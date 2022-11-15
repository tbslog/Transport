using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.ContractModel
{
    public class EditContract
    {
        public string SoHopDongCha { get; set; }
        public int NgayThanhToan { get; set; }
        public string TenHienThi { get; set; }
        public string MaPtvc { get; set; }
        public string PhanLoaiHopDong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; }
        public IFormFile File { get; set; }
        public string? PhuPhi { get; set; }
        public int TrangThai { get; set; }
    }
}
