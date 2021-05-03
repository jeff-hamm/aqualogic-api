using AqualogicJumper.Model.Status;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AqualogicJumper.Services
{
    public class StatusUpdateService
    {
        private readonly ILogger<StatusUpdateService> _log;
        private readonly SwitchService _switchService;
        private readonly IEnumerable<IUpdateProcessor> _processors;
        private readonly PoolStatusStore _store;
        public static Regex DisplayText { get; } = new Regex(@"^\s*((?<Line>([^\s]+\s?)+)\s*)+", RegexOptions.Compiled);
        public StatusUpdateService(
            ILogger<StatusUpdateService> log,
            MenuService menuService,
            SensorService sensorService,
            SettingService settingService,
            SwitchService switchService,
            PoolStatusStore store) 
        {
            _log = log;
            this._switchService = switchService;
            _processors = new IUpdateProcessor[]
            {
                menuService,
                sensorService,
                settingService
            };
            _store = store;
        }

        public bool TryProcess(string text)
        {
            var r = false;
            var linesMatch = DisplayText.Match(text);
            if (linesMatch.Success)
                text = String.Join("\n", linesMatch.Groups["Line"].Captures.Select(c => c.Value.Trim()));
            var update = new StatusUpdate()
            {
                Timestamp = DateTime.UtcNow,
                Text = text.IsFlashing(out var flashing),
                IsFlashing = flashing
            };
            _log.LogDebug($"Display update: {update}");
            foreach (var processor in _processors)
                r |= processor.TryProcess(update);
            _store.Status.DisplayText = update;
            _store.History.Text.Add(update);
            _store.SaveChanges();
            return r;
        }

        public bool TryProcess(SwitchName states, SwitchName flashingStates) =>
            _switchService.TryProcess(states, flashingStates);
    }
}