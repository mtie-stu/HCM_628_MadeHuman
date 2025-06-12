using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class ComboItem
    {
        [Key]
        public Guid ComboItemId { get; set; }

        public int Quantity { get; set; }

        // Foreign keys
        public int ComboId { get; set; }
        public int ProductId { get; set; }

        // Navigation properties
        public Combo Combo { get; set; }
        public Product Product { get; set; }
    }
}
