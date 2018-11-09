using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace TemplatedConfiguration.Tests
{
    public class TemplatedConfigurationSourceTests
    {
        private readonly IConfigurationRoot _configurationRoot;

        public TemplatedConfigurationSourceTests()
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
                            {"CaseInsensitiveSetting", "this comes from the default setting: '{defaultsetting}'"},
                            {"Recursive", "this is {Recursive}"},
                        })
                ).Build();
        }


        [Theory]
        [InlineData("SettingWithoutTemplate")] // Correctly cased
        [InlineData("settingwithouttemplate")] // Proves Case Insensititivity
        public void Can_get_value_without_template(string key)
        {
            var result = _configurationRoot.GetValue<string>(key);

            Assert.Equal("normal value", result);
        }


        [Theory]
        [InlineData("TemplatedSetting")] // Correctly cased
        [InlineData("templatedsetting")] // Proves Case Insensititivity on root template name
        [InlineData("CaseInsensitiveSetting")] // Proves Case Insensititivity on recursive template
        public void Can_get_templated_default_value(string key)
        {
            var result = _configurationRoot.GetValue<string>(key);

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
