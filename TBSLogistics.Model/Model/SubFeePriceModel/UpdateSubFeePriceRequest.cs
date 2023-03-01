using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubFeePriceModel
{
    public class UpdateSubFeePriceRequest
    {
        public string VehicleType { get; set; }
        public string CusType { get; set; }
        public string ContractId { get; set; }
        public long SfId { get; set; }
        public string GoodsType { get; set; }
        public int? AreaID { get; set; }
        public string TripID { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
    }
}
