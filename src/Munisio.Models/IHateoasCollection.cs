using System.Collections.Generic;

namespace Munisio.Models
{
    public interface IHateoasCollection
    {
        IEnumerable<IHateoasObject> Items { get; }
    }
}
