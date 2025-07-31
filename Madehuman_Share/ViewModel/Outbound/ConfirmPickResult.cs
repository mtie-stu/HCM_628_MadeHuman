using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class ConfirmPickResult
    {
        public List<string> Messages { get; set; } = new();
        public bool IsPickTaskFinished { get; set; }
        public object? NextDetail { get; set; }
    }
}
