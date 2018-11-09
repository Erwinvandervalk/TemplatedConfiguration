# Templated Configuration Source for Microsoft.Extensions.Configuration  [![Build Status](https://travis-ci.org/Erwinvandervalk/TemplatedConfiguration.svg?branch=master)](https://travis-ci.org/Erwinvandervalk/TemplatedConfiguration)

This library is an extension for Microsoft.Extensions.Configuration, which helps you to do recursive templating in configuration settings. 

When retrieving a configuration setting value, it will look for {placeholders}. If it finds any placeholder, it will try to see if the placeholder is actually configured as a setting. It will then (recursively) replace any placeholders within the setting's value. 

This means you can compose configuration settings out of other configuration settings, which will allow you to override only parts of a config setting or the entire setting.

## Example use cases.

For my settings, I like to use smart defaults and compose other settings from these smart defaults. 

For example, often when I have multiple components that require a database connection, then most of these components can use the same connection string. But I like to use default database names, so the only thing I really want to be 'forced' to configure is the server name. But, I still want the capability to override individual parts of the settings if I really choose to.

Another example is when accessing an URL from code, where the URL is usually composed of a 'base path' and a route. I want to use smart defaults, but retain the capability to override them if I need to.

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

## Licencing

Licenced under [MIT](https://opensource.org/licenses/MIT).



