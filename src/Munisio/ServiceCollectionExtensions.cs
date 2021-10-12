using Microsoft.AspNetCore.Mvc;

namespace Munisio
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Minusio's HATEOAS middleware to the MVC pipeline.
        /// </summary>
        public static MvcOptions AddHateoas(this MvcOptions builder)
        {
            builder.Filters.Add<HateoasActionFilter>();
            return builder;
        }
    }
}
