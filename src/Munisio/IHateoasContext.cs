using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Munisio
{
    public interface IHateoasContext
    {
        HttpContext HttpContext { get; }
        ClaimsPrincipal User { get; }
        LinkGenerator LinkGenerator { get; }
        IAuthorizationService AuthorizationService { get; }

        Task<bool> AuthorizeAsync(object resource, params IAuthorizationRequirement[] requirement);
    }
}
