using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory.Shared
{
    public class ProductionDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public int Qty { get; set; }
    }
}
