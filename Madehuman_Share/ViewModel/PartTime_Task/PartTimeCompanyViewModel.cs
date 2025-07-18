using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Madehuman_User.ViewModel.PartTime_Task
{
    public enum StatusPart_Time_Companyvm
    {
        Active,
        Inactive
    }
    public class PartTimeCompanyViewModel
    {
        public Guid? Id { get; set; } // null nếu thêm mới
        public string Name { get; set; }
        public string? Address { get; set; }
        public StatusPart_Time_Companyvm Status { get; set; }
    }

}
