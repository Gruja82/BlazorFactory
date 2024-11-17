using System.ComponentModel.DataAnnotations;

namespace Factory.Api.Data.Entities
{
    public class Production
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Qty { get; set; }
        public virtual Product Product { get; set; } = default!;
    }
}
