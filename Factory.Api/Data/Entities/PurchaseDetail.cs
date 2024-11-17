using System.ComponentModel.DataAnnotations;

namespace Factory.Api.Data.Entities
{
    public class PurchaseDetail
    {
        [Key]
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int MaterialId { get; set; }
        [Required]
        public int Qty { get; set; }
        public virtual Purchase Purchase { get; set; } = default!;
        public virtual Material Material { get; set; } = default!;
    }
}
