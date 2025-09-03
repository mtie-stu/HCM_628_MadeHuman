using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class AssignDispatchTaskResultViewModel
    {
        public List<string> Logs { get; set; } = new();
        public string? Message { get; set; }
    }

}
