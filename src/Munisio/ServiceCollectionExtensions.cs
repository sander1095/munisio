using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Munisio
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Munisio's HATEOAS middleware to the MVC pipeline.
        /// Don't forget to call <see cref="AddHateoasProviders(IServiceCollection)"/> or <see cref="AddHateoasProviders(IServiceCollection, Assembly[])"/> afterwards if you want Munisio to automatically register your HATEOAS providers!
        /// </summary>
        public static MvcOptions AddHateoas(this MvcOptions builder)
        {
            builder.Filters.Add<HateoasActionFilter>();
            return builder;
        }

        /// <summary>
        /// Searches for your <see cref="IHateoasProvider{TModel}"/> and <see cref="IAsyncHateoasProvider{TModel}"/> implementations in your current assembly and registers them in a transient manner to the <paramref name="services"/> you pass in.
        /// Don't forget to call <see cref="AddHateoas(MvcOptions)"/> so your providers will get executed at runtime!
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>
        /// Calling this method is not required; it's simply convenient. If you don't want Munisio to search and register your implementations automatically, feel free to register them yourself!
        /// </remarks>
        public static IServiceCollection AddHateoasProviders(this IServiceCollection services) => services.AddHateoasProviders(Assembly.GetExecutingAssembly());

        /// <summary>
        /// Searches for your <see cref="IHateoasProvider{TModel}"/> and <see cref="IAsyncHateoasProvider{TModel}"/> implementations in the <paramref name="assemblies"/> you pass in to this method. 
        /// They will be registered transient manner to the <paramref name="services"/> you pass in.
        /// 
        /// Don't forget to call <see cref="AddHateoas(MvcOptions)"/> so your providers will get executed at runtime!
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>
        /// Calling this method is not required; it's simply convenient. If you don't want Munisio to search and register your implementations automatically, feel free to register them yourself!
        /// </remarks>
        public static IServiceCollection AddHateoasProviders(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                throw new ArgumentException("No assemblies given to scan");
            }

            var providedAssemblies = assemblies.Distinct()
                                               .ToArray();

            var targetHateoasInterfaces = new Type[] { typeof(IHateoasProvider<>), typeof(IAsyncHateoasProvider<>) };

            foreach (var targetHateoasInterface in targetHateoasInterfaces)
            {
                var amountGenericTypeArguments = targetHateoasInterface.GetGenericArguments().Length;

                var typesToRegister = providedAssemblies.SelectMany(assembly => assembly.DefinedTypes)
                                                        .Where(x => x.GetImplementedHateoasInterfaceTypes(targetHateoasInterface).Any())
                                                        .Where(x => x.IsClass && !x.IsAbstract)
                                                        .Select(x => new { @interface = x.ImplementedInterfaces.First(y => y.GetGenericTypeDefinition() == targetHateoasInterface), definedType = x })
                                                        .ToList();

                foreach (var type in typesToRegister)
                {
                    services.AddTransient(targetHateoasInterface.MakeGenericType(type.@interface.GenericTypeArguments), type.definedType);
                }
            }
            return services;
        }

        private static IEnumerable<Type> GetImplementedHateoasInterfaceTypes(this Type type, Type targetType)
        {
            if (type is null) yield break;

            foreach (var interfaceType in type.GetInterfaces().Where(type => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == targetType))
            {
                yield return interfaceType;
            }
        }
    }
}
