using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AqualogicJumper.Model;
using AqualogicJumper.Model.Status;
using Microsoft.Extensions.Logging;

namespace AqualogicJumper.Services
{
    public class SettingService : IUpdateProcessor
    {

        private readonly ILogger<SettingService> _log;
        private readonly MenuService _menuService;
        private readonly PoolStatusStore _status;
        private readonly CommandService _commandService;

        public SettingService(ILogger<SettingService> log, MenuService menuService, PoolStatusStore status,
            CommandService commandService)
        {
            _log = log;
            _menuService = menuService;
            _status = status;
            _commandService = commandService;
        }
        public SettingValue GetValue(string name)
        {
            if (_status.Status.Settings.TryGetValue(name, out var value))
            {
                return value;
            }

            if (value == null && _menuService.TryFindSetting(name, out var setting))
            {
//                _menuService.SelectSetting(setting);
                return new SettingValue()
                {
                    Value = null,
                    LastModified = null,
                    Item = setting
                };
            }

            return null;
        }

        public ConcurrentDictionary<string, SettingValue> Pending { get; } = new ConcurrentDictionary<string, SettingValue>();
        public SettingValue SetValue(string name, string value = null)
        {
            var current = GetValue(name);
            if (current == null)
            {
                _log.LogWarning($"Unable to find key corresponding to switch name {name}");
                return null;
            }
            if (current.Value?.Equals(value, StringComparison.InvariantCultureIgnoreCase) == true) return current;
            _commandService.UpdateSetting(current.Item, value);
            return current;

        }

        public bool TryProcess(StatusUpdate update)
        {
            if (_menuService.CurrentMenu?.Setting == null) return false;
            var setting = _menuService.CurrentMenu.Setting;

            var value = update.Text;
            if(!String.IsNullOrEmpty(setting.Format))
                value = setting.Test.Replace(update.Text, setting.Format);
            var v = _status.Status.Settings.AddOrUpdate(setting.Name,
                name =>
                {
                    _log.LogDebug($"Creating setting {setting.Name}={value}");
                    return new SettingValue()
                    {
                        Item = setting,
                        Value = value,
                        LastModified = DateTime.UtcNow
                    };
                },
                (name, existing) =>
                {
                    _log.LogDebug($"Updating setting {setting.Name}={value}");
                    existing.Item = setting;
                    existing.LastModified = DateTime.UtcNow;
                    existing.Value = value;
                    return existing;
                });
            if (v.LastModified.HasValue && (!_status.LastSavedTime.HasValue ||
                                            _status.LastSavedTime < v.LastModified))
                _status.SaveChanges();

            foreach (var p in Pending.ToArray())
                if (v.Item.Name.Equals(p.Key, StringComparison.InvariantCultureIgnoreCase) && p.Value.Value?.Equals(v.Value, StringComparison.InvariantCultureIgnoreCase) == true)
                    Pending.TryRemove(p.Key, out _);
            return true;
        }


    }
}