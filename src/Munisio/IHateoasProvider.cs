using Munisio.Models;

namespace Munisio
{
    public interface IHateoasProvider<in TModel> where TModel : IHateoasObject
    {
        void Enrich(IHateoasContext context, TModel model);
    }
}
