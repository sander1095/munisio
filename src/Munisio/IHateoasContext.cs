using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Munisio
{
    public interface IHateoasContext
    {
        HttpContext HttpContext { get; }
        ClaimsPrincipal User { get; }
        IUrlHelper Url { get; }
        IAuthorizationService AuthorizationService { get; }

        Task<bool> AuthorizeAsync(object resource, params IAuthorizationRequirement[] requirement);
    }
}
