using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class CreateInboundTaskViewModel
    {
        [Required]
        public Guid InboundReceiptId { get; set; }

        //[Required]
        //public string? CreateBy { get; set; }
    }
}
