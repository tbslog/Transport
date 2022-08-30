using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Service.Repository.Common
{
    public interface ICommon
    {
        Task Log(string FileName, string LogMessage);
    }
}
