using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace TemplatedConfiguration
{
    public class TemplatedConfigurationProvider : IConfigurationProvider
    {
        private static readonly Regex _regex = new Regex(@"(\{[\w,\-,\.]*\})", RegexOptions.Compiled);
        private readonly IConfigurationRoot _source;

        /// <summary>Initialize a new instance from the source.</summary>
        /// <param name="source">The source settings.</param>
        public TemplatedConfigurationProvider(IConfigurationRoot source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            _source = source;
        }

        public bool TryGet(string key, out string value)
        {
            return TryGetInternal(key, new HashSet<TemplatedSettingKey>(), out value);
        }

        private bool TryGetInternal(TemplatedSettingKey key, HashSet<TemplatedSettingKey> visited, out string value)
        {
            value = _source[key.Name];
            if (value == null)
                return false;

            if (visited.Contains(key))
            {
                return false;
            }

            visited.Add(key);

            //var groups = _regex.Matches(value)
            //    .Select(x => x.Groups[1])
            //    .Where(x => !string.IsNullOrEmpty(x.Value))
            //    .ToArray();

            //foreach (var group in groups)
            //{
            //    if (TryGetInternal(group.Value, visited, out var groupValue))
            //    {
            //        value = value.Replace(group.Value, groupValue, StringComparison.OrdinalIgnoreCase);
            //    }
            //}

            return true;
        }

        public void Set(string key, string value)
        {
            _source[key] = value;
        }

        public IChangeToken GetReloadToken()
        {
            return _source.GetReloadToken();
        }

        public void Load()
        {
            _source.Reload();
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string path)
        {
            return _source.Providers.Aggregate(Enumerable.Empty<string>(),
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
