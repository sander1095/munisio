using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Munisio
{
    public class HateoasContext : IHateoasContext
    {
        private readonly ActionContext _context;
        private IUrlHelper _urlHelper = null!;

        public HttpContext HttpContext => _context.HttpContext;
        public ClaimsPrincipal User => _context.HttpContext.User;
        public IUrlHelper Url => LazyInitializer.EnsureInitialized(ref _urlHelper, () => new UrlHelper(_context));
        public IAuthorizationService AuthorizationService { get; }


        public HateoasContext(ActionContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            AuthorizationService = authorizationService;
        }

        public async Task<bool> AuthorizeAsync(object resource, params IAuthorizationRequirement[] requirement)
        {
            return (await AuthorizationService.AuthorizeAsync(User, resource, requirement).ConfigureAwait(false)).Succeeded;
        }
    }
}
