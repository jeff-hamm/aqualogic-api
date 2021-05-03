using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AqualogicJumper.Model.Status;
using Cyotek.Collections.Generic;
using JsonFlatFileDataStore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace AqualogicJumper.Services
{
    public class PoolStatusStore
    {
        public const string StatusKey = "poolStatusStore";
        public const string HistoryKey = "poolHistory";
        private readonly DataStore _store;
        private readonly ILogger<PoolStatusStore> _log;

        public PoolStatusStore(DataStore store, ILogger<PoolStatusStore> log)
        {
            _store = store;
            _log = log;
            Status = _store.Refresh<PoolStatus>(StatusKey);
            History = _store.Refresh<PoolStatusHistory>(HistoryKey);
            LastSavedTime = DateTime.UtcNow;
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
