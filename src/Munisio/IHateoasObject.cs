using System.Collections.Generic;

namespace Munisio
{
    public interface IHateoasObject
    {
        ICollection<HateoasLink> Links { get; }
    }
}
