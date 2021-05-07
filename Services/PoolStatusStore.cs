using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AqualogicJumper.Model;
using AqualogicJumper.Model.Status;
using Cyotek.Collections.Generic;
using JsonFlatFileDataStore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AqualogicJumper.Services
{
    public class PoolStatusStore
    {
        public const string StatusKey = "poolStatusStore";
        public const string HistoryKey = "poolHistory";
        private readonly DataStore _store;
        private readonly ILogger<PoolStatusStore> _log;
        private readonly string _menuFilePath;
        public const string MenuFile = "MenuMap.json";
        private readonly JsonSerializer _serializer;

        public PoolStatusStore(DataStore store, IWebHostEnvironment env, ILogger<PoolStatusStore> log, JsonSerializer serializer)
        {
            _store = store;
            _log = log;
            _serializer = serializer;
            LastSavedTime = DateTime.UtcNow;
            _menuFilePath = Path.Combine(env.WebRootPath, MenuFile);
            RefreshMenu();
            Status = _store.Refresh<PoolStatus>(StatusKey);
            foreach(var s in Status.SettingValues)
                FixSettingValue(s);
            foreach (var s in Status.StatusValues)
                FixSettingValue(s);
            History = _store.Refresh<PoolStatusHistory>(HistoryKey);
        }

        private void FixSettingValue<TValue>(IStatusValue<TValue> sv) where TValue:MenuValue
        {

            foreach (var menu in Menus)
                foreach (var s in menu.Children)
                    if (s.Name.Equals(sv.Item.Name, StringComparison.InvariantCultureIgnoreCase) && s is TValue set)
                        sv.Item = set;
        }

        public MenuMap Menu { get; private set; }
        private Sensor[] _sensors;
        public IEnumerable<Sensor> Sensors => _sensors ??= Menus.SelectMany(m => m.Children.OfType<Sensor>()).ToArray();
        public IEnumerable<Menu> Menus => Menu.Menus;
        public IEnumerable<Switch> Switches => Menu.Switches;

        public MenuState DefaultMenu { get; private set; }

        public void RefreshMenu()
        {
            using var reader = File.OpenText(_menuFilePath);
            using var jsonReader = new JsonTextReader(reader);
            Menu = _serializer.Deserialize<MenuMap>(jsonReader);
            if (Menu == null) throw new ArgumentNullException(MenuFile);
            DefaultMenu = new MenuState(Menus.LastOrDefault(), null, new StatusUpdate());
            CurrentMenu = DefaultMenu;
        }


        public bool IsToggled(SwitchName stateVal) =>
            PendingSwitches.TryGetValue(stateVal, out var val) ? val.Value : ToggledSwitches.Contains(stateVal);

        public HashSet<SwitchName> ToggledSwitches => Status?.States;
        public ConcurrentDictionary<SwitchName, SwitchValue> PendingSwitches { get; } = new();

        public PoolStatusHistory History { get; private set; }
        public PoolStatus Status { get; private set; }
        public DateTime? LastSavedTime { get; private set; }
        public MenuState CurrentMenu { get; set; }
        public int SaveChanges()
        {
            var changes = 0;
            if (_store.ReplaceItem(StatusKey, Status, true)) changes++;
            if (_store.ReplaceItem(HistoryKey, Status, true)) changes++;
            LastSavedTime = DateTime.UtcNow;
            return changes;
        }

    }
}
