﻿using System.ComponentModel.DataAnnotations;

namespace Factory.Api.Data.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = default!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = default!;
    }
}
