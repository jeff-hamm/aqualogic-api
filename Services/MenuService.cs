using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AqualogicJumper.Model;
using AqualogicJumper.Model.Status;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AqualogicJumper.Services
{
    public class MenuService : IUpdateProcessor
    {
        public const string MenuFile = "MenuMap.json";
        private readonly ILogger<MenuService> _log;
        private readonly JsonSerializer _serializer;
        private readonly CommandService _commandService;
        private readonly PoolStatusStore _store;
        private readonly string  _filePath;
        public static TimeSpan MaxAge { get; } = new TimeSpan(0, 15, 0);
        public static TimeSpan ScanInterval { get; } = new TimeSpan(0, 5, 0);

        public MenuService(ILogger<MenuService> log, JsonSerializer serializer, 
            IWebHostEnvironment env, CommandService commandService, 
            PoolStatusStore store)
        {
            _log = log;
            _serializer = serializer;
            _commandService = commandService;
            _store = store;
            _filePath = Path.Combine(env.WebRootPath, MenuFile);
            Refresh();
        }

        public void Refresh()
        {
            using var reader = File.OpenText(_filePath);
            using var jsonReader = new JsonTextReader(reader);
            Menu =_serializer.Deserialize<MenuMap>(jsonReader);
            if(Menu == null) throw new ArgumentNullException(MenuFile);
            DefaultMenu = new MenuState(Menus.LastOrDefault(), null, new StatusUpdate());
            CurrentMenu = DefaultMenu;
        }

        public MenuMap Menu { get; private set; }
        public IEnumerable<Sensor> Sensors => DefaultMenu.Menu.Children.OfType<Sensor>();
        public IEnumerable<Menu> Menus => Menu.Menus;
        public IEnumerable<Switch> Switches => Menu.Switches;

        public MenuState DefaultMenu { get; private set; }
        public MenuState CurrentMenu { get => _store.CurrentMenu; set => _store.CurrentMenu = value; }
        public DateTime? LastScan { get; private set; }
        public Queue<(Key,Regex)> CurrentPath { get; set; }

        public bool TryProcess(StatusUpdate update)
        {
            Scan(update);
            foreach (var menu in Menus)
            {

                if (menu.TryMatch(update, out var match))
                {
                    SetMenu(new MenuState(menu, match, update));
                    return true;
                }
            }
            var newSetting = Menus.OrderByDescending(m => m == CurrentMenu.Menu)
                .ThenByDescending(m => m == DefaultMenu.Menu)
                .Select(menu => menu.Children
                    .Select(setting => setting.TryMatch(update, out var match) ? new MenuState(setting, match, update) : null)
                    .FirstOrDefault(sm => sm != null))
                .FirstOrDefault(sm => sm != null);
            if (newSetting == null)
            {
                SetMenu(DefaultMenu);
                return false;
            }

            SetMenu(newSetting);
            return true;
        }
        private void SetMenu(MenuState state)
        {
            if (CurrentMenu?.Menu != state?.Menu || CurrentMenu?.Setting != state?.Setting || CurrentMenu?.Text?.Text != state?.Text.Text)
            {
                _log.LogDebug(
                    $"Changing CurrentMenu from  {CurrentMenu?.Menu?.Name}/{CurrentMenu?.Setting?.Name} to {state?.Menu?.Name}/{state?.Setting?.Name}");
                CurrentMenu = state;
            }
        }

        private readonly ConcurrentDictionary<Menu, DateTime> _lastScanTime = new ConcurrentDictionary<Menu, DateTime>();
        private void Scan(StatusUpdate update)
        {
            var toScan = new HashSet<Menu>();
            foreach (var menu in Menus)
            {
                foreach (var setting in menu.Children)
                {
                    if (!_store.Status.Settings.TryGetValue(setting.Name, out var value))
                        continue;

                    if (!value.LastModified.HasValue || DateTime.UtcNow.Subtract(value.LastModified.Value) > MaxAge)
                    {
                        _store.Status.Settings.TryRemove(value.Item.Name, out _);
                        _store.SaveChanges();
                        toScan.Add(menu);
                    }   
                }

                if (!_lastScanTime.TryGetValue(menu, out var scan) || DateTime.UtcNow.Subtract(scan) > ScanInterval)
                    toScan.Add(menu);


            }

            foreach (var menu in toScan)
                ScanMenu(menu);


        }
        
        private void ScanMenu(Menu menu)
        {
            if (_lastScanTime.TryGetValue(menu, out var scan) && DateTime.UtcNow.Subtract(scan) < ScanInterval)
            {
                return;
            }

            _lastScanTime.AddOrUpdate(menu, DateTime.UtcNow, (m, t) => DateTime.UtcNow);
            _commandService.Select(menu.Children.Last());
        }
        
        public bool TryFindSetting(string name, out Setting setting)
        {
            foreach (var menu in Menus)
            {
                foreach (var s in menu.Children)
                {
                    if (s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && s is Setting set)
                    {
                        setting = set;
                        return true;
                    }
                }
            }

            setting = null;
            return false;
        }
    }
}
