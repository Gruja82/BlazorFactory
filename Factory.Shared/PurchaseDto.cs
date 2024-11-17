﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory.Shared
{
    public class PurchaseDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public string SupplierName { get; set; } = string.Empty;
        public List<PurchaseDetailDto> PurchaseDetailList { get; set; } = new();
    }
}