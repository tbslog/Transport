using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NguoiDung
    {
        public NguoiDung()
        {
           
        }

        public int Id { get; set; }
        public string HoVaTen { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MaNhanVien { get; set; }
        public string MaBoPhan { get; set; }
        public string MaCapBac { get; set; }
        public int Role { get; set; }
        public int TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual User User { get; set; }
    
    }
}
