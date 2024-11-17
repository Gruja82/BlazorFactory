using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory.Shared
{
    public class OrderDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderDetailDto> OrderDetailsList { get; set; } = new();
    }
}
