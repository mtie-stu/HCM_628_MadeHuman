using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Shop
{
    public class Product_Combo_Img
    {
        [Key]
        public Guid Id { get; set; } = default!;

        public Guid? ProductId { get; set; }
        public Guid? ComboId { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        [ForeignKey("ProductId")]
        [JsonIgnore]
        public virtual Product Product { get; set; }

        [ForeignKey("ComboId")]
        [JsonIgnore]
        public virtual Combo Combo { get; set; }
    }
}
                         