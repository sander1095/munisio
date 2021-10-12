using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Munisio
{
    public class HateoasLinkBuilder
    {
        private readonly IHateoasObject _target;
        private HateoasLink? _link;

        public HateoasLinkBuilder(
            IHateoasObject target,
            string rel,
            string route,
            string method)
        {
            _target = target;

            if (target.Links.All(p => p.Rel.Equals(rel, StringComparison.OrdinalIgnoreCase)))
            {
                _link = new HateoasLink(rel, route, method);
                _target.Links.Add(_link);
            }
        }

        public HateoasLinkBuilder When(Func<bool> predicate)
        {
            if (_link is not null && !predicate())
            {
                RemoveLink();
            }

            return this;
        }

        public async Task<HateoasLinkBuilder> WhenAsync(Func<Task<bool>> predicate)
        {
            if (_link is not null && !await predicate().ConfigureAwait(false))
            {
                RemoveLink();
            }

            return this;
        }

        public HateoasLinkBuilder ForRoles(ClaimsPrincipal principal, params string[] roles)
        {
            if (_link is not null && !IsApplicableForUser(principal, roles))
            {
                RemoveLink();
            }

            return this;


            static bool IsApplicableForUser(ClaimsPrincipal principal, params string[] roles)
            {
                if (roles is null)
                {
                    return true;
                }

                return roles.Any(p => principal.IsInRole(p.ToString()));
            }
        }

        private HateoasLinkBuilder RemoveLink()
        {
            if (_link is not null)
            {
                _target.Links.Remove(_link);
                _link = null;
            }

            return this;
        }
    }
}
