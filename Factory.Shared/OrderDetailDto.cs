using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory.Shared
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        [Required]
        public string OrderCode { get; set; } = string.Empty;
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public int Qty { get; set; }
    }
}
