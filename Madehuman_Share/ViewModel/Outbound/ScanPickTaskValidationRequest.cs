using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class ScanPickTaskValidationRequest
    {
        public Guid PickTaskId { get; set; }
        public Guid PickTaskDetailId { get; set; }
        public string WareHouseLocation { get; set; }   
        public string SKU { get; set; } = string.Empty;
        public Guid? BasketId { get; set; } // ✅ mới thêm

    }

}
