using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Munisio
{
    public class HateoasContext : IHateoasContext
    {
        private readonly ActionContext _context;

        public HttpContext HttpContext => _context.HttpContext;
        public ClaimsPrincipal User => _context.HttpContext.User;

        public LinkGenerator LinkGenerator { get; }
        public IAuthorizationService AuthorizationService { get; }

        public HateoasContext(ActionContext context, IAuthorizationService authorizationService, LinkGenerator linkGenerator)
        {
            _context = context;
            AuthorizationService = authorizationService;
            LinkGenerator = linkGenerator;
        }

        public async Task<bool> AuthorizeAsync(object resource, params IAuthorizationRequirement[] requirement)
        {
            return (await AuthorizationService.AuthorizeAsync(User, resource, requirement).ConfigureAwait(false)).Succeeded;
        }
    }
}
