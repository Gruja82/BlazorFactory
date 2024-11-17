using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory.Shared
{
    public class Pagination<T>
    {
        public IList<T> DataList { get; set; } = default!;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 4;
        public int TotalPages { get; set; }
    }
}
