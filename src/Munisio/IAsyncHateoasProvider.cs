using System.Threading.Tasks;
using Munisio.Models;

namespace Munisio
{
    public interface IAsyncHateoasProvider<in TModel> where TModel : IHateoasObject
    {
        Task EnrichAsync(IHateoasContext context, TModel model);
    }
}
