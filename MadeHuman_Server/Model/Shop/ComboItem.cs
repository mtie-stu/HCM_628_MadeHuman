using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.Shop
{
    public class ComboItem
    {
        [Key]
        public Guid ComboItemId { get; set; } = default!;

        public int Quantity { get; set; } = default!;

        // Foreign keys
        public Guid ComboId { get; set; } = default!;
        public Guid ProductId { get; set; } = default!;

        // Navigation properties

        public Combo Combo { get; set; }
        public Product Product { get; set; }
    }
}
