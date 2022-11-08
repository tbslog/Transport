using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.UserModel
{
    public class GetUserRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string HoVaTen { get; set; }
        public string MaNhanVien { get; set; }
        public string MaBoPhan { get; set; }
        public string TenBoPhan { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string StatusName { get; set; }
        public string TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
