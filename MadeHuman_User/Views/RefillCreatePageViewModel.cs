using Madehuman_Share.ViewModel.Inbound;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadeHuman_User.Views
{
    public class RefillCreatePageViewModel
    {
        public RefillTaskFullViewModel Task { get; set; } = new();

        public List<SelectListItem> FromOptions { get; set; } = new();
        public List<SelectListItem> ToOptions { get; set; } = new();
    }
}
