using System;

namespace AqualogicJumper.Services
{
    public class SwitchValue : IEquatable<SwitchValue>
    {
        public DateTime? LastModified { get; set; }
        public SwitchName Name { get; set; }
        public bool Value { get; set; }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(SwitchValue other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return (int) Name;
        }

        public static bool operator ==(SwitchValue left, SwitchValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SwitchValue left, SwitchValue right)
        {
            return !left.Equals(right);
        }
    }
}