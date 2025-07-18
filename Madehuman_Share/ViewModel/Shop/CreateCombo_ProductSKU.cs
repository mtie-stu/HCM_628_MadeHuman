using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Madehuman_User.ViewModel.Shop
{
    public class CreateComboWithItemsViewModel
    {
        public Guid ComboId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [NotMapped]
        [Display(Name = "Chọn Hình")]
        public List<IFormFile>? ImageFiles { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [NotMapped]
        public string? ItemsJson { get; set; } // ✅ Gửi từ FE dạng JSON string

        [NotMapped]
        public List<ComboItemInputModel> Items { get; set; } = new();
    }

/*    public class ComboItemInputModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }*/
    public class CreateComboViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        [NotMapped]
        public List<IFormFile>? ImageFiles { get; set; }
    }
    public class AddComboItemsRequest
    {
        public Guid ComboId { get; set; }
        public List<ComboItemInputModel> Items { get; set; }
    }

    public class ComboItemInputModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }


}
