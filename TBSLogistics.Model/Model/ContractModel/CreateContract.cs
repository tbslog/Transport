using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.ContractModel
{
    public class CreateContract
    {
        public string MaHopDong { get; set; }
        public string Account { get; set; }
        public string SoHopDongCha { get; set; }
        public string LoaiHinhHopTac { get; set; }
        public string MaLoaiSPDV { get; set; }
        public string MaLoaiHinh { get; set; }
        public string HinhThucThue { get; set; }
        public string TenHienThi { get; set; }
        public string MaKh { get; set; }
        public int? NgayThanhToan { get; set; }
        public string PhanLoaiHopDong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; }
        public IFormFile FileContract { get; set; }
        public IFormFile FileCosting { get; set; }
        public string PhuPhi { get; set; }
        public int TrangThai { get; set; }
    }
}
