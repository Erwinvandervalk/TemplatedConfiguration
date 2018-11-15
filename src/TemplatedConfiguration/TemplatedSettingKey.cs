using System;

namespace TemplatedConfiguration
{
    public class TemplatedSettingKey : IEquatable<TemplatedSettingKey>
    {
        public bool Equals(TemplatedSettingKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
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
            if (String.IsNullOrEmpty(name))
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