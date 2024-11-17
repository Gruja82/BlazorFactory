using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Factory.Api.Data.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(name: "Price", TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public virtual Category Category { get; set; } = default!;
        public virtual ICollection<ProductDetail> ProductDetails { get; set; } = default!;
        public virtual ICollection<Production> Productions { get; set; } = default!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = default!;
    }
}
