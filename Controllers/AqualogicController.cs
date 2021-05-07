using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AqualogicJumper.Model.Status;
using AqualogicJumper.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AqualogicJumper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AqualogicController : ControllerBase
    {

        private readonly ILogger<AqualogicController> _logger;
        private readonly AquaLogicProtocolService _service;
        private readonly SwitchService _switchService;
        private readonly SensorService _sensorService;
        private readonly SettingService _settingService;
        private readonly PoolStatusStore _store;

        public AqualogicController(ILogger<AqualogicController> logger,
            AquaLogicProtocolService service, SwitchService switchService,
            SensorService sensorService, SettingService settingService,
            PoolStatusStore store)
        {
            _logger = logger;
            _service = service;
            _switchService = switchService;
            _sensorService = sensorService;
            _settingService = settingService;
            _store = store;
        }

        [HttpGet]
        public PoolStatusBase Get(string name = null)
        {
            if (!String.IsNullOrEmpty(name))
            {
                if (_store.Status.Values.TryGetValue(name, out SensorValue v))
                {
                    return new PoolStatusBase()
                    {
                        Timestamp = _store.Status.Timestamp,
                        StatusValues = new[] {v},
                    };
                }
                if (Enum.TryParse(name, true, out SwitchName stateVal))
                {
                    return new PoolStatusBase()
                    {
                        Timestamp = _store.Status.Timestamp,
                        States = new HashSet<SwitchName>()
                        {
                            _store.IsToggled(stateVal) ? stateVal : SwitchName.NONE
                        }
                    };
                }
            }

            return _store.Status;
        }

        [HttpGet("History")]
        public PoolStatusHistory History()
        {
            return _store.History;
        }
        [HttpGet("Switch/{id}")]
        public bool? GetSwitch(string id,bool? value = null)
        {
            _logger.LogDebug($"HttpGet Switch/{id}");
            if (Enum.TryParse(id, true, out SwitchName stateVal))
            {
                if(value.HasValue)
                    _switchService.Toggle(stateVal, value);
                return _store.IsToggled(stateVal);
            }

            return null;
        }

        [HttpPost("Switch/{id}")]
        public bool? Switch(string id, [FromBody]bool? value=null)
        {
            
            _logger.LogDebug($"HttpPost Switch/{id}, value={value}");
            if (Enum.TryParse(id, true, out SwitchName stateVal)) {
                _switchService.Toggle(stateVal, value);
                return _store.IsToggled(stateVal);
            }

            return null;
        }

        [HttpGet("Sensor/{id}")]
        public SensorValue Sensor(string id)
        {
            _logger.LogDebug($"HttpGet Sensor/{id}");
            return _sensorService?.GetValue(id);
        }


        [HttpGet("Setting/{id}")]
        public SettingValue GetSetting(string id, string value = null)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _logger.LogDebug($"HttpPost Setting/{id}, value={value}");
                _settingService.SetValue(id, value);
            }
            _logger.LogDebug($"HttpGet Setting/{id}");
            return _settingService?.GetValue(id);
        }



        [HttpPost("Setting/{id}")]
        public SettingValue Setting(string id, [FromBody] string value = null)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _logger.LogDebug($"HttpPost Setting/{id}, value={value}");
                _settingService.SetValue(id, value);
            }
            _logger.LogDebug($"HttpGet Setting/{id}");
            return _settingService?.GetValue(id);
        }

    }
}
