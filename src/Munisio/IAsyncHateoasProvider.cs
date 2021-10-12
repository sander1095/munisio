using System.Threading.Tasks;

namespace Munisio
{
    public interface IAsyncHateoasProvider<in TModel> where TModel : IHateoasObject
    {
        Task EnrichAsync(IHateoasContext context, TModel model);
    }
}
