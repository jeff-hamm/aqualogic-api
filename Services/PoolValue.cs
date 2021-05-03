using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AqualogicJumper.Services
{
    public class PoolValue
    {
        public int Id => (int) Name;
        [JsonConverter(typeof(StringEnumConverter))]
        public PoolValueName Name { get; set; }
        public string Text { get; set; }
        public object Value { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ValueUnit? Unit { get; set; }
        public bool Modified { get; set; }
    }
}