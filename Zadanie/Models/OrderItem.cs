using System.ComponentModel.DataAnnotations.Schema;

namespace Zadanie.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public string Name { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public int OrderId { get; set; }
    }
}
