using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AqualogicJumper.Services
{
    public static class Extensions
    {
        public static IEnumerable<TEnum> Flags<TEnum>(this TEnum input) where TEnum : Enum
        {
            foreach (TEnum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        public static string ToHexString(this byte[] ba)
        {
            var hex = new StringBuilder("0x", ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        private static readonly ConcurrentDictionary<(Type enumType, Enum val, Type attribute), Attribute> _attributeCache = 
            new ConcurrentDictionary<(Type enumType, Enum val, Type attribute), Attribute>();

        public static IDictionary<TEnum, TAttr> Attributes<TEnum, TAttr>() 
            where TEnum:Enum 
            where TAttr:Attribute =>
            Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToDictionary(v => v, 
                v => v.Attribute<TAttr>());

        public static TAttribute Attribute<TAttribute>(this Enum @this)
            where TAttribute : Attribute
        {
            var enumType = @this.GetType();
            return _attributeCache.GetOrAdd((enumType, @this,typeof(TAttribute)), key => {
                var enumValue = Enum.GetName(key.enumType, key.val);
                MemberInfo member = enumType.GetMember(enumValue)[0];
                return member.GetCustomAttributes(key.attribute, false).FirstOrDefault() as Attribute;
            }) as TAttribute;
        }

        public static byte[] ToLittleEndian(this short val)
        {
            var dest = new byte[2];
            BinaryPrimitives.WriteInt16LittleEndian(dest, val);
            return dest;
        }
        public static byte[] ToLittleEndian(this int val)
        {
            var dest = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(dest, val);
            return dest;
        }
        public static byte[] ToBigEndian(this short val)
        {
            var dest = new byte[2];
            BinaryPrimitives.WriteInt16BigEndian(dest, val);
            return dest;
        }
        public static byte[] ToBigEndian(this int val)
        {
            var dest = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(dest, val);
            return dest;
        }

        public const byte FlashingCharacterIndicator = 0b10000000;
        public static string IsFlashing(this string msg, out bool flashing)
        {
            flashing = false;
            var copy = new char[msg.Length];
            for (int i = 0; i < msg.Length; i++)
            {
                if (((byte) msg[i] & FlashingCharacterIndicator) > 0)
                {
                    flashing = true;
                    copy[i] = (char) ((byte) msg[i] ^ FlashingCharacterIndicator);
                }
                else
                    copy[i] = msg[i];
            }

            return flashing ? new string(copy) : msg;
        }
        public static string GetNullTerminatedSTring(this Encoding @this, ReadOnlySpan<byte> buffer)
        {
            int count = buffer.IndexOf((byte)0);
            if (count < 0) count = buffer.Length;
            return @this.GetString(buffer[..count]);
        }

    }
}
