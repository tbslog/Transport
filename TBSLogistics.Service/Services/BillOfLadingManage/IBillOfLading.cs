using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.BillOfLadingManage
{
    public interface IBillOfLading
    {
        Task<LoadDataTransPort> getListRoadBillOfLading(string RoadId);
    }
}
