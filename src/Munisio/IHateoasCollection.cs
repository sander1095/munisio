using System.Collections.Generic;

namespace Munisio
{
    public interface IHateoasCollection
    {
        IEnumerable<IHateoasObject> Items { get; }
    }
}
