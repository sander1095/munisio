using System.Collections.Generic;

namespace Munisio
{
    public class HateoasObject : IHateoasObject
    {
        public ICollection<HateoasLink> Links { get; } = new List<HateoasLink>();
    }
}
