using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace TemplatedConfiguration.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                // First set up defaults
                .AddInMemoryCollection(Global.DefaultSettings)
                .AddInMemoryCollection(ComponentA.DefaultSettings)
                .AddInMemoryCollection(ComponentB.DefaultSettings)

                // Then set up the 'overrides'. 
                .AddIniFile("Config.ini")
                .AddCommandLine(args)
                .AddEnvironmentVariables()

                // Wrap the configuration providers with the provider that supports templating
                .WithRecursiveTemplateSupport()

                .Build();

                Console.WriteLine("Component A: Connection String: " +config.GetValue<string>("ComponentA.ConnectionString"));
            Console.WriteLine("Component B: Connection String: " +config.GetValue<string>("ComponentB.ConnectionString"));

            Console.ReadLine();
        }

        public class MySEttings
        {
            public string SettingA { get; set; }
        }

        public class Global
        {
            public static readonly Dictionary<string, string> DefaultSettings = new Dictionary<string, string>()
            {
                {"settinga", "abc" },
                // Default global settings. 
                {"Global.ConnectionString.Security", "Integrated Security=true;" },
                {"Global.ConnectionString.Settings", "" },
            };
        }

        public class ComponentA
        {
            public static readonly Dictionary<string, string> DefaultSettings = new Dictionary<string, string>()
            {
                // The connection string is composed from parts
                // But can also be overwritten as a whole
                {"ComponentA.ConnectionString", "{ComponentA.ConnectionString.Security}Server={ComponentA.ConnectionString.ServerName};Database={ComponentA.ConnectionString.DatabaseName}{ComponentA.ConnectionString.Settings}"},

                // Each part can be overwritten per 'component' or globally
                {"ComponentA.ConnectionString.Security", "{Global.ConnectionString.Security}" },
                {"ComponentA.ConnectionString.ServerName", "{Global.ConnectionString.ServerName}" },
                {"ComponentA.ConnectionString.Settings", "{Global.ConnectionString.Settings}" },

                // Some default settings are simply hard coded, such as the default database name
                {"ComponentA.ConnectionString.DatabaseName", "ComponentA" },
            };
        }
        public class ComponentB
        {
            public static readonly Dictionary<string, string> DefaultSettings = new Dictionary<string, string>()
            {
                // The connection string is composed from parts
                // But can also be overwritten as a whole
                {"ComponentB.ConnectionString", "{ComponentB.ConnectionString.Security}Server={ComponentB.ServerName};Database={ComponentB.DatabaseName}{ComponentB.ConnectionString.Settings}"},

                // Each part can be overwritten per 'component' or globally
                {"ComponentB.ConnectionString.Security", "{Global.ConnectionString.Security}" },
                {"ComponentB.ConnectionString.ServerName", "{Global.ConnectionString.ServerName}" },
                {"ComponentB.ConnectionString.Settings", "{Global.ConnectionString.Settings}" },

                // Some default settings are simply hard coded, such as the default database name
                {"ComponentB.ConnectionString.DatabaseName", "ComponentA" },
            };
        }
    }
}
