using System.Security.Claims;
using Munisio.Models;

namespace Munisio
{
    public static class HateoasExtensions
    {
        public static HateoasLinkBuilder ForRoles(this HateoasLinkBuilder builder, ClaimsPrincipal user, params string[] roles) =>
            builder.ForRoles(user, roles);

        public static HateoasLinkBuilder AddLink(this IHateoasObject target, string rel, string route) =>
            new(target, rel, route, "GET");

        public static HateoasLinkBuilder AddPatchLink(this IHateoasObject target, string rel, string route) =>
            new(target, rel, route, "PATCH");

        public static HateoasLinkBuilder AddPostLink(this IHateoasObject target, string rel, string route) =>
            new(target, rel, route, "POST");

        public static HateoasLinkBuilder AddPutLink(this IHateoasObject target, string rel, string route) =>
            new(target, rel, route, "PUT");

        public static HateoasLinkBuilder AddDeleteLink(this IHateoasObject target, string rel, string route) =>
            new(target, rel, route, "DELETE");
    }
}
