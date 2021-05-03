using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AqualogicJumper.Model;
using AqualogicJumper.Model.Status;
using Microsoft.Extensions.Logging;

namespace AqualogicJumper.Services
{
    public class SwitchService
    {
        private readonly ILogger<SwitchService> _log;
        private readonly PoolStatusStore _status;
        private readonly CommandService _commandService;
        private IEnumerable<Switch> _buttons;

        public SwitchService(ILogger<SwitchService> log, IEnumerable<Switch> buttons, 
            PoolStatusStore status, CommandService commandService)
        {
            _log = log;
            _status = status;
            _commandService = commandService;
            _buttons = buttons;
        }
        public HashSet<SwitchName> Flashing
        {
            get => _status.Status.FlashingStates;
            set => _status.Status.FlashingStates = value;
        }
        public bool TryProcess(SwitchName onSwitches, SwitchName flashingStates)
        {
            var switches = (flashingStates | onSwitches).Flags().ToHashSet();
            var changed = SetSwitches(switches,_status.ToggledSwitches);
            foreach(var p in  _status.PendingSwitches.ToArray())
                if (_status.ToggledSwitches.Contains(p.Key) == p.Value.Value)
                    _status.PendingSwitches.TryRemove(p.Key,out _);
            if(SetSwitches(flashingStates.Flags().ToHashSet(), Flashing) && changed)
                _status.SaveChanges();
            return true;
        }

        private bool SetSwitches(HashSet<SwitchName> switches, HashSet<SwitchName> toggled)
        {
            var changed = toggled.RemoveWhere(n => {
                var r = !switches.Contains(n);
                if (r)
                    _log.LogDebug($"Turned off: {n}");
                return r;
            }) > 0;
            foreach (var on in switches)
            {
                if (!toggled.Contains(on))
                {
                    toggled.Add(on);
                    changed = true;
                    _log.LogInformation($"Turned on {on}");
                }
            }

            return changed;
        }
        public void Toggle(SwitchName stateVal, bool? value=null) =>
            _commandService.Toggle(stateVal, value);
    }
}