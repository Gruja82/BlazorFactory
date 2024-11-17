using System.ComponentModel.DataAnnotations;

namespace Factory.Api.Data.Entities
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; } = default!;
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = default!;
    }
}
