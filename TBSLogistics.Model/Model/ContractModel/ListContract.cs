using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.ContractModel
{
    public class ListContract
    {
        public List<ListContract> listAddendums { get; set; }
        public string ChuoiKhachHang { get; set; }
        public string Account { get; set; }
        public string LoaiHinhHopTac { get; set; }
        public string MaLoaiSPDV { get; set; }
        public string MaLoaiHinh { get; set; }
        public string HinhThucThue { get; set; }
        public int? NgayThanhToan { get; set; }
        public string MaHopDong { get; set; }
        public string MaBangGia { get; set; }
        public string TenHienThi { get; set; }
        public string SoHopDongCha { get; set; }
        public string MaKh { get; set; }
        public string TenKH { get; set; }
        public string PhanLoaiHopDong { get; set; }
        public string FileContract { get; set; }
        public string FileCosing { get; set; }
        public string TrangThai { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
    }
}
