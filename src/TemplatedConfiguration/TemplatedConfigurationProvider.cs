﻿using System;
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

    public class TemplatedConfigurationProvider : IConfigurationProvider
    {
        private static readonly Regex _regex = new Regex(@"(\{[\w,\-,\.]*\})", RegexOptions.Compiled);
        public readonly IConfigurationRoot InnerConfiguration;

        /// <summary>Initialize a new instance from the source.</summary>
        /// <param name="innerConfiguration">The source settings.</param>
        public TemplatedConfigurationProvider(IConfigurationRoot innerConfiguration)
        {
            if (innerConfiguration == null)
                throw new ArgumentNullException(nameof(innerConfiguration));
            InnerConfiguration = innerConfiguration;
        }

        public bool TryGet(string key, out string value)
        {
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

        public void Set(string key, string value)
        {
            InnerConfiguration[key] = value;
        }

        public IChangeToken GetReloadToken()
        {
            return InnerConfiguration.GetReloadToken();
        }

        public void Load()
        {
            InnerConfiguration.Reload();
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string path)
        {
            return InnerConfiguration.Providers.Aggregate(Enumerable.Empty<string>(),
                (seed, source) =>
                    source.GetChildKeys(seed, path)).Distinct().Select(
                key =>
                {
                    string key1;
                    if (path != null)
                        key1 = ConfigurationPath.Combine(path, key);
                    else
                        key1 = key;
                    return key1;
                });
        }

        private class TemplatedSettingKey : IEquatable<TemplatedSettingKey>
        {
            public bool Equals(TemplatedSettingKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TemplatedSettingKey)obj);
            }

            public override int GetHashCode()
            {
                return (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
            }

            public static bool operator ==(TemplatedSettingKey left, TemplatedSettingKey right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TemplatedSettingKey left, TemplatedSettingKey right)
            {
                return !Equals(left, right);
            }

            public readonly string Name;

            public TemplatedSettingKey(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("message", nameof(name));
                }

                Name = name.TrimStart('{').TrimEnd('}')?.ToLower();
            }

            public static implicit operator string(TemplatedSettingKey key)
            {
                return key.ToString();
            }

            public static implicit operator TemplatedSettingKey(string name)
            {
                return new TemplatedSettingKey(name);
            }

            public override string ToString()
            {
                return "{" + Name + "}";
            }
        }
    }
}
