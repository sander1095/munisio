using System.Collections.Generic;

namespace Munisio.Models
{
    public class HateoasCollection<T> : HateoasObject, IHateoasCollection where T : IHateoasObject
    {
        public IEnumerable<T> Items { get; }

        IEnumerable<IHateoasObject> IHateoasCollection.Items => (IEnumerable<IHateoasObject>)Items;

        public HateoasCollection(IEnumerable<T> items) => Items = items;
    }

    public static class HateoasCollection
    {
        public static HateoasCollection<T> ForItems<T>(IEnumerable<T> items) where T : IHateoasObject =>
            new HateoasCollection<T>(items);

        public static PagedHateoasCollection<T> ForPagedItems<T>(IEnumerable<T> items, int page, int pageSize, int totalAmountOfPages, int totalAmountOfItems) where T : IHateoasObject =>
            new PagedHateoasCollection<T>(items, page, pageSize, totalAmountOfPages, totalAmountOfItems);
    }
}
