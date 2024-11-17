using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Factory.Api.Modules
{
    public class QueryStringParamsOrder
    {
        [FromQuery]
        public string? SearchText { get; set; }
        [FromQuery]
        public string? StringDate  { get; set; }
        [FromQuery]
        public string? Customer { get; set; }
        [FromQuery]
        public int PageIndex { get; set; }
        [FromQuery]
        public int PageSize { get; set; }
    }
}
