using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.Shop
{
    public class ComboItem
    {
        [Key]
        public Guid ComboItemId { get; set; }

        public int Quantity { get; set; }

        // Foreign keys
        public Guid ComboId { get; set; }
        public Guid ProductId { get; set; }

        // Navigation properties

        public Combo Combo { get; set; }
        public Product Product { get; set; }
    }
}
