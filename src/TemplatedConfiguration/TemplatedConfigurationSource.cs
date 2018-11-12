using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TemplatedConfiguration
{
    public class TemplatedConfigurationSource : IConfigurationSource
    {
        private readonly ConfigurationBuilder _innerConfigurationBuilder;

        public TemplatedConfigurationSource(IConfigurationBuilder builder)
        {
            _innerConfigurationBuilder = new ConfigurationBuilder();
            for (int i = 0; i < builder.Sources.Count; i++)
            {
                var source = builder.Sources[i];
                if (source == this)
                {
                    // We only care about providers before this provider. So stop looking after we've found it. 
                    break;
                }
                
                // We wrap the providerSource into a proxied provider source, that caches the built output, so that we don't build 'twice'. 
                var proxiedSource = new ProxiedProviderSource(source);
                builder.Sources[i] = proxiedSource;
                _innerConfigurationBuilder.Add(proxiedSource);
            }
        }

        public TemplatedConfigurationSource(Action<IConfigurationBuilder> configurer)
        {
            _innerConfigurationBuilder = new ConfigurationBuilder();
            configurer(_innerConfigurationBuilder);
        }


        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
             return new TemplatedConfigurationProvider(_innerConfigurationBuilder.Build());
        }
    }

    internal class ProxiedProviderSource : IConfigurationSource
    {
        private readonly IConfigurationSource _source;

        private IConfigurationProvider _provider;

        public ProxiedProviderSource(IConfigurationSource source)
        {
            _source = source;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (_provider != null)
                return _provider;

            return _provider = _source.Build(builder);
        }
    }


}