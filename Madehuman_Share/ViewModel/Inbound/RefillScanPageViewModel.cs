using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class RefillScanPageViewModel
    {
        public RefillTaskFullViewModel Task { get; set; } = new();
        public ScanRefillTaskValidationRequest ScanRequest { get; set; } = new();
        public RefillTaskDetailWithHeaderViewModel TaskDetailFlat { get; set; } = new();
    }
}
