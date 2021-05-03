using System.Collections.Generic;
using AqualogicJumper.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AqualogicJumper.Model
{
    public abstract class MenuValue : MenuNode
    {
        public override IEnumerable<MenuValue> Children { get; set; } = new MenuValue[0];
    }
    public class Setting : MenuValue
    {
        protected override bool ReadOnly => false;
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueUnit? Unit { get; set; }
        public ICollection<Key> Inputs { get; set; }
    }
}