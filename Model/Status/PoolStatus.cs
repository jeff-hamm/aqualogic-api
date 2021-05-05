using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AqualogicJumper.Services;
using Newtonsoft.Json;

namespace AqualogicJumper.Model.Status
{
    public class PoolStatusBase
    {
        public DateTime Timestamp { get; set; }

        private HashSet<SwitchName> _states;
        public HashSet<SwitchName> States
        {
            get => _states ??= new HashSet<SwitchName>();
            set => _states = value;
        } 
        public virtual IEnumerable<SensorValue> StatusValues { get; set; }
        public virtual IEnumerable<SettingValue> SettingValues { get; set; }
    }
    public class PoolStatus : PoolStatusBase
    {
        public StatusUpdate DisplayText { get; set; }
        private HashSet<SwitchName> _flashingStates;
        public HashSet<SwitchName> FlashingStates
        {
            get => _flashingStates ??= new HashSet<SwitchName>();
            set => _flashingStates = value;
        }


        [JsonIgnore]
        internal ConcurrentDictionary<string, SensorValue> Values { get; private set; }
            = new ConcurrentDictionary<string, SensorValue>();
        public override IEnumerable<SensorValue> StatusValues
        {
            get => Values.Values;
            set
            {
                Values.Clear();
                foreach (var v in value)
                {
                    Values.AddOrUpdate(v.Item.Name, v, (_, __) => v);
                }
            }
        }
        [JsonIgnore]
        internal ConcurrentDictionary<string,SettingValue> Settings {get; private set; } 
            = new ConcurrentDictionary<string, SettingValue>();

        public override IEnumerable<SettingValue> SettingValues
        {
            get => Settings.Values;
            set
            {
                Settings.Clear();
                foreach (var v in value)
                {

                    Settings.AddOrUpdate(v.Item.Name, v, (_, __) => v);
                }
            }
        }

    }
}