using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AqualogicJumper.Model;
using AqualogicJumper.Model.Status;
using Microsoft.Extensions.Logging;

namespace AqualogicJumper.Services
{
    public class SensorService : IUpdateProcessor
    {
        private readonly ILogger<SensorService> _log;
        private readonly PoolStatusStore _status;
        private IEnumerable<Sensor> _sensors;
        public static TimeSpan MaxSensorAge { get; } = new TimeSpan(0, 5, 0);

        public SensorService(ILogger<SensorService> log, IEnumerable<Sensor> sensors, PoolStatusStore status)
        {
            _log = log;
            _status = status;
            _sensors = sensors;
        }
        public SensorValue GetValue(string name)
        {
            if (_status.Status.Values.TryGetValue(name, out var value))
            {
                if (value.LastModified.HasValue && DateTime.UtcNow.Subtract(value.LastModified.Value) < MaxSensorAge)
                    return value;
                _status.Status.Values.TryRemove(name, out _);
            }

            return null;
        }

        public bool TryProcess(StatusUpdate update)
        {
            var processed = false;
            foreach (var sensor in _sensors)
            {
                if (!sensor.TryMatch(update, out var m)) continue;
                var value = sensor.Test.Replace(update.Text, sensor.Format);
                var v = _status.Status.Values.AddOrUpdate(sensor.Name, 
                    name => new SensorValue()
                    {
                        Item = sensor,
                        Value = value,
                        LastModified = DateTime.UtcNow
                    },
                    (name, existing) =>
                    {
                        existing.Item = sensor;
                        existing.LastModified = DateTime.UtcNow;
                        existing.Value = value;
                        return existing;
                    });
                if (v.LastModified.HasValue && (!_status.LastSavedTime.HasValue ||
                                                _status.LastSavedTime < v.LastModified)) 
                    _status.SaveChanges();

                processed = true;
            }

            return processed;
        }
    }
}