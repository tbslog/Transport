using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;

namespace TBSLogistics.Model.Model.ContractModel
{
    public class GetContractById
    {
        public string Account { get; set; }
        public string LoaiHinhHopTac { get; set; }
        public string LoaiSPDV { get; set; }
        public string LoaiHinhKho { get; set; }
        public string HinhThucThueKho { get; set; }
        public int? NgayThanhToan { get; set; }
        public string MaHopDong { get; set; }
        public string SoHopDongCha { get; set; }
        public string TenHienThi { get; set; }
        public string MaKh { get; set; }
        public string MaPtvc { get; set; }
        public string PhanLoaiHopDong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public string FileContract { get; set; }
        public string FileCosing { get; set; }
    }
}
