using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel
{

    public enum UserStatus
    {
        InActive,
        Active,
        Banned

    }
    public class BulkRegisterModel
    {
        public UserStatus UserStatus { get; set; }  
        public int Quantity { get; set; } // Số lượng tài khoản cần tạo
        public string DefaultPassword { get; set; } = "MadeHuman@123";
    }

}
