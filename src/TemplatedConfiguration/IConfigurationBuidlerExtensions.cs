using System;
using Microsoft.Extensions.Configuration;

namespace TemplatedConfiguration
{
    public static class IConfigurationBuidlerExtensions
    {
        public static IConfigurationBuilder WithRecursiveTemplateSupport(this IConfigurationBuilder builder, Action<IConfigurationBuilder> configurer)
        {
            builder.Add(new TemplatedConfigurationSource(configurer));
            return builder;
        }

        public static IConfigurationBuilder WithRecursiveTemplateSupport(this IConfigurationBuilder builder)
        {
            builder.Add(new TemplatedConfigurationSource(builder));
            return builder;
        }
    }
}