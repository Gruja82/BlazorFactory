using Microsoft.AspNetCore.Mvc;

namespace Factory.Api.Modules
{
    public class QueryStringParamsProduction
    {
        [FromQuery]
        public string? SearchText { get; set; }
        [FromQuery]
        public string? StringDate { get; set; }
        [FromQuery]
        public string? ProductName { get; set; }
        [FromQuery]
        public int PageIndex { get; set; }
        [FromQuery]
        public int PageSize { get; set; }
    }
}
