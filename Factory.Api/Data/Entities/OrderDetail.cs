using System.ComponentModel.DataAnnotations;

namespace Factory.Api.Data.Entities
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        [Required]
        public int Qty { get; set; }
        public virtual Product Product { get; set; } = default!;
        public virtual Order Order { get; set; } = default!;
    }
}
