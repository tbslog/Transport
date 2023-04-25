using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PriceListModel
{
    public class ApprovePriceTable
    {
        public List<Result> Result { get; set; }
    }

    public class Result
    {
        public long Id { get; set; }
        public int IsAgree { get; set; }
    }
}
