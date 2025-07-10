using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Madehuman_Share.ViewModel.Shop
{
    public class CreateProduct_ProdcutSKU_ViewModel
    {
        [BindNever]
        public Guid ProductId { get; set; }
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? SKU { get; set; }
        public int QuantityInStock { get; set; }
        [NotMapped]
        [Display(Name = "Chọn Hình")]
        public List<IFormFile>? ImageFiles { get; set; } // Cho phép upload nhiều ảnh
        // Foreign key
        [BindNever]
        public Guid Id { get; set; }//ProductSKUId

        public Guid CategoryId { get; set; }
        // ✅ Dùng để hiển thị tên danh mục
        public string? CategoryName { get; set; }
        [BindNever]
        public Guid ProductItemId { get; set; }

    }
}
