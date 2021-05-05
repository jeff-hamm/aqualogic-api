using System.Collections.Generic;
using System.Text.RegularExpressions;
using AqualogicJumper.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JsonSubTypes;
namespace AqualogicJumper.Model
{
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(Menu), MenuItemType.Menu)]
    [JsonSubtypes.KnownSubType(typeof(Sensor), MenuItemType.Sensor)]
    [JsonSubtypes.KnownSubType(typeof(Setting), MenuItemType.Setting)]
    public abstract class MenuNode
    {
        public abstract IEnumerable<MenuValue> Children { get; set; }
        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        public Menu Menu { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual MenuItemType Type { get; set; }
        public string Format { get; set; }
        [JsonConverter(typeof(RegexConverter))]
        public Regex Test { get; set; }
        public virtual bool TryMatch(StatusUpdate update, out Match match)
        {
            if (IsFlashing.HasValue && IsFlashing != update.IsFlashing)
            {
                match = null;
                return false;
            }
            match = Test.Match(update.Text);
            return match.Success;
        }
        public bool? IsFlashing { get; set; }

        protected virtual bool ReadOnly { get; } = true;

    }

}