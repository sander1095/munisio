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
        /// Adds Minusio's HATEOAS middleware to the MVC pipeline.
        /// </summary>
        public static MvcOptions AddHateoas(this MvcOptions builder)
        {
            builder.Filters.Add<HateoasActionFilter>();
            return builder;
        }

        public static IServiceCollection AddHateoas(this IServiceCollection services) => services.AddHateoas(Assembly.GetExecutingAssembly());

        public static IServiceCollection AddHateoas(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                throw new ArgumentException("No assemblies given to scan");
            }

            var providedAssemblies = assemblies.Distinct()
                                               .ToArray();

            var targetHateoasInterfaces = new Type[] {
                typeof(IHateoasProvider<>)
                , typeof(IAsyncHateoasProvider<>)
            };

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

            foreach (var interfaceType in type.GetInterfaces()
                                              .Where(type => type.GetTypeInfo().IsGenericType
                                                                   && type.GetGenericTypeDefinition() == targetType))
            {
                yield return interfaceType;
            }
        }
    }
}
