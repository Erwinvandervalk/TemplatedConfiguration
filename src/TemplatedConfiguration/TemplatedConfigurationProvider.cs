using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace TemplatedConfiguration
{
    internal static class StringExtensions
    {
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }

    public class TemplatedConfigurationProvider : ConfigurationProvider
    {
        private static readonly Regex _regex = new Regex(@"(\{[\w,\-,\.:]*\})", RegexOptions.Compiled);
        public readonly IConfigurationRoot InnerConfiguration;

        /// <summary>Initialize a new instance from the source.</summary>
        /// <param name="innerConfiguration">The source settings.</param>
        public TemplatedConfigurationProvider(IConfigurationRoot innerConfiguration)
        {
            InnerConfiguration = innerConfiguration;
        }

        public override bool TryGet(string key, out string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                value = null;
                return false;
            }
            return TryGetInternal(key, new HashSet<TemplatedSettingKey>(), out value);
        }

        private bool TryGetInternal(TemplatedSettingKey key, HashSet<TemplatedSettingKey> visited, out string value)
        {
            value = InnerConfiguration[key.Name];
            if (value == null)
                return false;

            if (visited.Contains(key))
            {
                return false;
            }

            visited.Add(key);
            var matches = _regex.Matches(value).Cast<Match>();

            var groups = matches
                .Select(x => x.Groups[1])
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .ToArray();

            foreach (var group in groups)
            {
                if (TryGetInternal(group.Value, visited, out var groupValue))
                {
                    value = value.Replace(group.Value, groupValue, StringComparison.OrdinalIgnoreCase);
                }
            }

            return true;
        }
    }
}
