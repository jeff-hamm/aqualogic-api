using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AqualogicJumper.Services
{
    public class PoolValueAttribute : Attribute
    {
        public PoolValueAttribute(){}

        public PoolValueAttribute(string name, ValueUnit unit)
        {
            Name = name;
            Unit = unit;
        }
        public string Name { get; set; }
        public ValueUnit Unit { get; set; }

        public static string ToValueName(PoolValueName value) =>
            PoolValues.TryGetValue(value, out var a) && !String.IsNullOrEmpty(a.Name) ? a.Name : value.ToString();

        public static IDictionary<PoolValueName, PoolValueAttribute> PoolValues { get; } =
            Extensions.Attributes<PoolValueName, PoolValueAttribute>();

    }
}