using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AqualogicJumper.Services
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PoolValueName
    {
        Unknown,
        // Pool Temp <value>F
        [PoolValue("Pool Temp", ValueUnit.Degrees)]
        PoolTemp,
        [PoolValue("Spa Temp", ValueUnit.Degrees)]
        SpaTemp,
        [PoolValue("Air Temp", ValueUnit.Degrees)]
        AirTemp,
        // Pool Chlorinator <value>%
        [PoolValue("Pool Chlorinator", ValueUnit.Percentage)]
        PoolChlorinator,
        [PoolValue("Spa Chlorinator", ValueUnit.Percentage)]
        SpaChlorinator,
        // Salt Level <value> [g/L|PPM|
        [PoolValue("Salt Level", ValueUnit.Concentration)]
        SaltLevel,
        // Check System <msg>
        [PoolValue("Check System", ValueUnit.String)]
        CheckSystem,
        [PoolValue("Heater1", ValueUnit.String)]
        HeatPump,
        // Heater1 Auto
        [PoolValue("Heater1", ValueUnit.AutoMode)]
        HeaterAuto,
    }
}