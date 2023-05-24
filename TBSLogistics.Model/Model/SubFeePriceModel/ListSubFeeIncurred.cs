using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubFeePriceModel
{
    public class ListSubFeeIncurred
    {
        public long Id { get; set; }
        public string DiaDiem { get; set; }
        public string MaVanDon { get; set; }
        public string MaSoXe { get; set; }
        public string TaiXe { get; set; }
        public string SubFee { get; set; }
        public string Payfor { get; set; }
        public double Price { get; set; }
        public string priceType { get; set; }
        public string Type { get; set; }
        public string TrangThai { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
