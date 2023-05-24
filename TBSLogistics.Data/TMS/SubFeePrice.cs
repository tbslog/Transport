using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class SubFeePrice
    {
        public SubFeePrice()
        {
            SubFeeByContract = new HashSet<SubFeeByContract>();
        }

        public long PriceId { get; set; }
        public string CusType { get; set; }
        public string ContractId { get; set; }
        public long SfId { get; set; }
        public string VehicleType { get; set; }
        public string GoodsType { get; set; }
        public double Price { get; set; }
        /// <summary>
        /// 0: deactivated, 1: create new, 2: approved, 3: deleted
        /// </summary>
        public byte SfStateByContract { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DeactiveDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Approver { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }
        public int? GetEmptyPlace { get; set; }
        public int? FirstPlace { get; set; }
        public int? SecondPlace { get; set; }
        public string AccountId { get; set; }
        public string PriceType { get; set; }

        public virtual AccountOfCustomer Account { get; set; }
        public virtual HopDongVaPhuLuc Contract { get; set; }
        public virtual SubFee Sf { get; set; }
        public virtual ICollection<SubFeeByContract> SubFeeByContract { get; set; }
    }
}
