using MadeHuman_Server.Model.Shop;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Inbound
{
    public class RefillTaskDetails
    {
        public Guid Id { get; set; }
        public Guid FromLocation { get; set; }
        public Guid ToLocation { get; set; }  
        public int Quantity { get; set; }
        public Guid RefillTaskId { get; set; }
        [ForeignKey("RefillTaskId")]
        public RefillTasks RefillTasks { get; set; } // FK
                                                     // Sửa lại chỗ này
        public Guid ProductSKUId { get; set; } = default!;


        [ForeignKey("ProductSKUId")]
        [JsonIgnore]
        public ProductSKU ProductSKUs { get; set; }


    }
}
