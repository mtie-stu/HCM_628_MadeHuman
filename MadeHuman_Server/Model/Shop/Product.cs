using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Shop
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Foreign key
        public Guid CategoryId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Category Category { get; set; }

        [JsonIgnore]
        public ICollection<ProductItem> ProductItems { get; set; }

        [JsonIgnore]
        public ICollection<ComboItem> ComboItems { get; set; }

        [JsonIgnore]
        public virtual ICollection<Product_Combo_Img> Product_Combo_Img { get; set; } = new List<Product_Combo_Img>();

        // 1-1 với ProductSKU
        public ProductSKU ProductSKU { get; set; }
    }
}
