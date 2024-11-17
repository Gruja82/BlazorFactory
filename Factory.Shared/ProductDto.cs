using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory.Shared
{
    public class ProductDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string CategoryName { get; set; } = string.Empty;
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        public List<ProductDetailDto> ProductDetailsList { get; set; } = new();
    }
}
