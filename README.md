# Templated Configuration Source for Microsoft.Extensions.Configuration

This library is an extension for Microsoft.Extensions.Configuration, which helps you to do recursive templating in configuration settings. 

When retrieving a configuration setting value, it will look for {placeholders}. If it finds any placeholder, it will try to see if the placeholder is actually configured as a setting. It will then (recursively) replace any placeholders within the setting's value. 

This means you can compose configuration settings out of other configuration settings, which will allow you to override only parts of a config setting or the entire setting.

## Getting Started

* Add a reference to TemplatedConfiguration
* Add a using statement to 'TemplatedConfiguration'

* Wrap the normal 'configuration builder' with a templated support builder, preferably by using the 
**WithRecursiveTemplateSupport** Extension method.

``` c#

    using TemplatedConfiguration;

    /* ... */

        var config = new ConfigurationBuilder()
            .WithRecursiveTemplateSupport(
                // Wrap the configuration providers with the provider that supports templating
                builder => builder
                    // First set up defaults
                    // Then set up the 'overrides'. 
                    .AddIniFile("Config.ini")
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                )
            .Build();
```
## Contributing

## Licencing




