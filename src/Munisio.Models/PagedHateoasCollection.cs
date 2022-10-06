using System.Collections.Generic;
using Munisio.Models;

namespace Munisio.Models
{
    public class PagedHateoasCollection<T> : HateoasCollection<T> where T : IHateoasObject
    {
        public int Page { get; }
        public int PageSize { get; }
        public int TotalAmountOfPages { get; }
        public int TotalAmountOfItems { get; }

        public PagedHateoasCollection(IEnumerable<T> items, int page, int pageSize, int totalAmountOfPages, int totalAmountOfItems) : base(items)
        {
            Page = page;
            PageSize = pageSize;
            TotalAmountOfPages = totalAmountOfPages;
            TotalAmountOfItems = totalAmountOfItems;
        }
    }
}
