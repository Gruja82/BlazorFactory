using System.ComponentModel.DataAnnotations;

namespace Factory.Api.Data.Entities
{
    public class ProductDetail
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MaterialId { get; set; }
        [Required]
        public int QtyMaterial { get; set; }
        public virtual Product Product { get; set; } = default!;
        public virtual Material Material { get; set; } = default!;
    }
}
