﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SFeeByTcommandModel
{
    public class CreateSFeeByTCommandRequest
    {
        public long IdTcommand { get; set; }
        public long SfId { get; set; }
        public double FinalPrice { get; set; }
        public string Note { get; set; }
    }
}
