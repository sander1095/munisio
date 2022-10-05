using System.Collections.Generic;

namespace Munisio.Models
{
    public class HateoasObject : IHateoasObject
    {
        public ICollection<HateoasLink> Links { get; } = new List<HateoasLink>();
    }
}
