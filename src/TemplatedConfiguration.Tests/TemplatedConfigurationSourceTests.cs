using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace TemplatedConfiguration.Tests
{
    public class TemplatedConfigurationSourceTests
    {

        public TemplatedConfigurationSourceTests()
        {

        }

        [Theory]
        [InlineData("SettingWithoutTemplate")] // Correctly cased
        [InlineData("settingwithouttemplate")] // Proves Case Insensititivity
        public void Can_get_value_without_template(string key)
        {
            var configurationRoot = BuildConfigurationRootWithoutRebuilding();

            var result = configurationRoot.GetValue<string>(key);

            Assert.Equal("normal value", result);
        }

        private static IConfigurationRoot BuildConfigurationRootWithoutRebuilding()
        {
            var configurationRoot = new ConfigurationBuilder()
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
            return configurationRoot;
        }


        [Theory]
        [InlineData("TemplatedSetting")] // Correctly cased
        [InlineData("templatedsetting")] // Proves Case Insensititivity on root template name
        [InlineData("CaseInsensitiveSetting")] // Proves Case Insensititivity on recursive template
        public void Can_get_templated_default_value(string key)
        {
            var configurationRoot = BuildConfigurationRootWithoutRebuilding();
            var result = configurationRoot.GetValue<string>(key);

            Assert.Equal("this comes from the default setting: '[default value]'", result);
        }

        [Fact]
        public void Can_handle_loops_in_recursion()
        {
            var configurationRoot = BuildConfigurationRootWithoutRebuilding();
            var result = configurationRoot.GetValue<string>("Recursive");

            Assert.Equal("this is {Recursive}", result);
        }

        [Theory]
        [InlineData("TemplatedSetting")] // Correctly cased
        [InlineData("templatedsetting")] // Proves Case Insensititivity on root template name
        [InlineData("CaseInsensitiveSetting")] // Proves Case Insensititivity on recursive template
        public void Can_add_templated_configuration_without_subbuilder(string key)
        {
            var configurationRoot = new ConfigurationBuilder()
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
                .WithRecursiveTemplateSupport()
                .Build();

            var result = configurationRoot.GetValue<string>(key);

            Assert.Equal("this comes from the default setting: '[default value]'", result);

        }

        [Fact]
        public void Build_is_only_called_once_on_source()
        {
            var fakeConfigSource = new FakeConfigSource();
            var configurationRoot = new ConfigurationBuilder()
                .Add(fakeConfigSource)
                .WithRecursiveTemplateSupport()
                .Build();

            Assert.Equal(1, fakeConfigSource.BuildCount);

        }

        private class FakeConfigSource : IConfigurationSource
        {
            public int BuildCount = 0;

            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                BuildCount++;
                return new FakeConfigurationProvider();
            }
        }
        private class FakeConfigurationProvider : ConfigurationProvider { }
    }
}
