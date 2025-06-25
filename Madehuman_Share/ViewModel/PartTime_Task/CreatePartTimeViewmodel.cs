using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.PartTime_Task
{
    public class CreatePartTimeViewModel
    {
        public string PartTimeId { get; set; }
        public string Name { get; set; }
        public string CCCD { get; set; }
        public string PhoneNumber { get; set; }
        public Guid CompanyId { get; set; }
    }
}
