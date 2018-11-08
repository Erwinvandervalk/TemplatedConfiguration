using System;
using Microsoft.Extensions.Configuration;

namespace TemplatedConfiguration
{
    public class TemplatedConfigurationSource : IConfigurationSource
    {
        private readonly IConfigurationRoot _config;

        public TemplatedConfigurationSource(Action<IConfigurationBuilder> configurer) : this(BuildConfigurationRoot(configurer))
        {

        }

        private static IConfigurationRoot BuildConfigurationRoot(Action<IConfigurationBuilder> configurerer)
        {
            var builder = new ConfigurationBuilder();
            configurerer(builder);
            return builder.Build();
        }

        public TemplatedConfigurationSource(IConfigurationRoot config)
        {
            _config = config;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new TemplatedConfigurationProvider(_config);
        }
    }
}