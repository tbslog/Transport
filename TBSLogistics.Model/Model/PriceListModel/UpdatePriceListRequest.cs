using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PriceListModel
{
   public class UpdatePriceListRequest
    {
    
        public DateTime NgayApDung { get; set; }
        public int TrangThai { get; set;}
        public List<BangGiaCungDuong> listPriceOfRoad { get; set; }
        public class BangGiaCungDuong
        {
            public int Id { get; set; }
            public string MaPtvc { get; set; }
            public string MaCungDuong { get; set; }
            public string MaLoaiPhuongTien { get; set; }
            public decimal GiaVnd { get; set; }
            public decimal GiaUsd { get; set; }
            public string MaDvt { get; set; }
            public string MaLoaiHangHoa { get; set; }
        }
    }
}
