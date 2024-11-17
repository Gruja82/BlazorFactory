using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    public class QueryStringParamsPurchase
    {
        [FromQuery]
        public string? SearchText { get; set; }
        [FromQuery]
        public string? StringDate { get; set; }
        [FromQuery]
        public string? Supplier { get; set; }
        [FromQuery]
        public int PageIndex { get; set; }
        [FromQuery]
        public int PageSize { get; set; }
    }
}
