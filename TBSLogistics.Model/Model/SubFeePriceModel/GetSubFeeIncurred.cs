using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubFeePriceModel
{
    public class GetSubFeeIncurred
    {
        public int? PlaceId { get; set; }
        public string MaVanDon { get; set; }
        public string TaiXe { get; set; }
        public string MaSoXe { get; set; }
        public string Romooc { get; set; }
        public string LoaiPhuongTien { get; set; }
        public string PhuPhi { get; set; }
        public double FinalPrice { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
