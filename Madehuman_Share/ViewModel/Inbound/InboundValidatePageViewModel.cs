using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class InboundValidatePageViewModel
    {
        public ScanInboundTaskValidationRequest ScanRequest { get; set; } = new();
        public GetInboundTaskById_Viewmodel? TaskInfo { get; set; }   // lấy từ service
    }

}
