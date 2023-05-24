using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class SfeeByTcommand
    {
        public long Id { get; set; }
        public long IdTcommand { get; set; }
        public long SfId { get; set; }
        public double Price { get; set; }
        public int ApproveStatus { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Approver { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }
        public int? PlaceId { get; set; }
        public int PayForId { get; set; }

        public virtual DieuPhoi IdTcommandNavigation { get; set; }
        public virtual SftPayFor PayFor { get; set; }
        public virtual SubFee Sf { get; set; }
    }
}
