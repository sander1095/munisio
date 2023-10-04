using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Munisio.Models;

namespace Munisio
{
    /// <summary>
    /// This filter will execute all registered <see cref="IHateoasProvider{TModel}"/>
    /// and <see cref="IAsyncHateoasProvider{TModel}"/> classes to enrich the model with Hateoas links.
    /// </summary>
    /// <remarks>
    /// There are sync and async variants of the "ApplyHateoasOnX" methods because we support <see cref="IHateoasProvider{TModel}"/> and <see cref="IAsyncHateoasProvider{TModel}"/>
    /// and the async version can't executed from a sync context.
    /// </remarks>
    internal class HateoasActionFilter : IAsyncActionFilter
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly LinkGenerator _linkGenerator;

        public HateoasActionFilter(IAuthorizationService authorizationService, LinkGenerator linkGenerator)
        {
            _authorizationService = authorizationService;
            _linkGenerator = linkGenerator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executed = await next().ConfigureAwait(false);

            if (executed.Result is not ObjectResult objectResult)
            {
                return;
            }

            if (objectResult.Value is IHateoasCollection collection)
            {
                if (collection is not null && collection.Items.Any())
                {
                    ApplyHateoasOnCollection(collection.Items, context);
                    await ApplyHateoasOnCollectionAsync(collection.Items, context).ConfigureAwait(false);
                }
            }

            if (objectResult.Value is IHateoasObject obj)
            {
                ApplyHateoasOnObject(obj, context);
                await ApplyHateoasOnObjectAsync(obj, context).ConfigureAwait(false);
            }
        }

        private void ApplyHateoasOnObject(IHateoasObject hateoasObject, ActionContext context)
        {
            var providerType = typeof(IHateoasProvider<>).MakeGenericType(hateoasObject.GetType());
            var provider = (dynamic?)context.HttpContext.RequestServices.GetService(providerType);

            if (provider is null)
            {
                return;
            }

            provider.Enrich(CreateHateoasContext(context), (dynamic)hateoasObject);
        }

        private async Task ApplyHateoasOnObjectAsync(IHateoasObject hateoasObject, ActionContext context)
        {
            var providerType = typeof(IAsyncHateoasProvider<>).MakeGenericType(hateoasObject.GetType());
            var provider = (dynamic?)context.HttpContext.RequestServices.GetService(providerType);

            if (provider is null)
            {
                return;
            }

            await ((Task)provider.EnrichAsync(CreateHateoasContext(context), (dynamic)hateoasObject)).ConfigureAwait(false);
        }

        private void ApplyHateoasOnCollection(IEnumerable<IHateoasObject> collection, ActionContext context)
        {
            var providerType = typeof(IHateoasProvider<>).MakeGenericType(collection.First().GetType());
            var provider = (dynamic?)context.HttpContext.RequestServices.GetService(providerType);

            if (provider is null)
            {
                return;
            }

            foreach (var item in collection)
            {
                provider.Enrich(CreateHateoasContext(context), (dynamic)item);
            }
        }

        private async Task ApplyHateoasOnCollectionAsync(IEnumerable<IHateoasObject> collection, ActionContext context)
        {
            var providerType = typeof(IAsyncHateoasProvider<>).MakeGenericType(collection.First().GetType());
            var provider = (dynamic?)context.HttpContext.RequestServices.GetService(providerType);

            if (provider is null)
            {
                return;
            }

            foreach (var item in collection)
            {
                await ((Task)provider.EnrichAsync(CreateHateoasContext(context), (dynamic)item)).ConfigureAwait(false);
            }
        }

        private HateoasContext CreateHateoasContext(ActionContext context) =>
            new(context, _authorizationService, _linkGenerator);
    }
}
