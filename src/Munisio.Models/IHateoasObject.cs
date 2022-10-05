using System.Collections.Generic;
using Munisio.Models;

namespace Munisio.Models
{
    public interface IHateoasObject
    {
        ICollection<HateoasLink> Links { get; }
    }
}
