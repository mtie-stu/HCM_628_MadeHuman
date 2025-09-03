using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class ConfirmPickDetailRequest
    {
        [Required]
        public Guid PickTaskId { get; set; }

        [Required]
        public Guid PickTaskDetailId { get; set; }

        [Required]
        public Guid BasketId { get; set; }
    }
}
