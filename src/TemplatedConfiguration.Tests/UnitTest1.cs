using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TemplatedConfiguration;
using Xunit;

namespace TemplatedConfigurationSource.Tests
{
    public class RecursiveSourceTests
    {
        private readonly IConfigurationRoot _configurationRoot;

        public RecursiveSourceTests()
        {
            _configurationRoot = new ConfigurationBuilder()
                .WithRecursiveTemplateSupport(
                    builder => builder
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {
                            {"SettingWithoutTemplate", "normal value"},
                            {"DefaultSetting", "[default value]"},
                        })
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {
                            {"TemplatedSetting", "this comes from the default setting: '{DefaultSetting}'"},
                            {"Recursive", "this is {Recursive}"},
                        })
                ).Build();
        }

        [Fact]
        public void Can_get_value_without_template()
        {
            var result = _configurationRoot.GetValue<string>("SettingWithoutTemplate");

            Assert.Equal("normal value", result);
        }


        [Fact]
        public void Can_get_templated_default_value()
        {
            var result = _configurationRoot.GetValue<string>("TemplatedSetting");

            Assert.Equal("this comes from the default setting: '[default value]'", result);
        }

        [Fact]
        public void Can_handle_loops_in_recursion()
        {
            var result = _configurationRoot.GetValue<string>("Recursive");

            Assert.Equal("this is {Recursive}", result);
        }
    }
}
