using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class DanhMuc
    {
        public DanhMuc()
        {
            NguoiDungTheoDanhMucs = new HashSet<NguoiDungTheoDanhMuc>();
        }

        public int Id { get; set; }
        public string TenDanhMuc { get; set; }
        public int DanhMucCha { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int TrangThai { get; set; }
        public int IdnguoiDung { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
        public virtual ICollection<NguoiDungTheoDanhMuc> NguoiDungTheoDanhMucs { get; set; }
    }
}
