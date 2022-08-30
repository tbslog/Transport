﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.DriverModel
{
    public class GetDriverRequest
    {
        public string MaTaiXe { get; set; }
        public string Cccd { get; set; }
        public string HoVaTen { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GhiChu { get; set; }
        public string MaNhaThau { get; set; }
        public string LoaiXe { get; set; }
    public bool TaiXeTBSL { get; set; }
        public string TrangThai { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime Createdtime { get; set; }
    }
}
