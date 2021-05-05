using AqualogicJumper.Model;
using AqualogicJumper.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AqualogicJumper.Services
{
    internal class CommandExecution
    {
        public DateTime EnqueueTime { get; } = DateTime.UtcNow;
        public DateTime? LastKeyTime { get; set; }
        public Key? LastKey { get; set; }
        public DateTime? CompletionTime { get; set; }
        public bool IsActive => LastKeyTime.HasValue;
        public bool IsComplete => CompletionTime.HasValue;
        public AqualogicCommand Command { get; set; }
    }

    public class CommandService
    {
        private readonly ILogger<CommandService> _log;
        private readonly AqualogicMessageWriter _writer;
        private readonly PoolStatusStore _store;
        private readonly ConcurrentQueue<CommandExecution> _commands = new ConcurrentQueue<CommandExecution>();
        public AqualogicCommand Current { get; private set; }

        public CommandService(ILogger<CommandService> log, AqualogicMessageWriter writer, PoolStatusStore store)
        {
            _log = log;
            _writer = writer;
            _store = store;
        }

        public async Task<bool> TryProcess(CancellationToken token)
        {

            if (!GetCurrentCommand(out var exec)) return false;

            // If we've already sent a command and not received any updates
            if (exec.LastKey.HasValue && exec.LastKeyTime.HasValue && _store.LastSavedTime < exec.LastKeyTime)
            {
                // resend
                if (DateTime.UtcNow.Subtract(exec.LastKeyTime.Value) > AqualogicMessageWriter.RateLimitDelay)
                {
                    if (await _writer.SendKey(exec.LastKey.Value, token))
                    {
                        exec.LastKey = exec.LastKey;
                        exec.LastKeyTime = DateTime.UtcNow;

                    }
                }

                //wait
                return false;
            }

            // Check that the command needs a key
            if (!exec.Command.HasNextKey(_store, out var key))
                return false;
            if (!(await _writer.SendKey(key, token)))
            {
                exec.LastKeyTime = null;
                return false;
            }

            _log.LogDebug($"Sending Key {key} for Command {exec.Command}");
            exec.LastKey = key;
            exec.LastKeyTime = DateTime.UtcNow;
            return true;
        }


        private bool GetCurrentCommand(out CommandExecution exec)
        {
            while (_commands.TryPeek(out exec))
            {
                // if this command is active, return it.
                if (!exec.IsComplete && !exec.Command.Complete)
                    return true;

                // If the command has already completed, remove it from the queue and loop
                if (exec.Command.Complete)
                    exec.CompletionTime = DateTime.UtcNow;
                _commands.TryDequeue(out _);
                _log.LogInformation($"[Complete]{exec.Command}");
            }

            return false;
        }

        private void Enqueue(AqualogicCommand cmd)
        {
            _commands.Enqueue(new CommandExecution()
            {
                Command = cmd,
            });
        }
        /*
        internal void SendKey(Key key, Func<bool> p)
        {
            _writer.SendKey(key, p);
        }*/

        internal void Select(MenuValue menuValue)
        {
            Enqueue(new MenuSelectCommand(menuValue));
        }

        public void Toggle(SwitchName stateVal, bool? value=null)
        {
            Enqueue(new ToggleSwitchCommand(stateVal, value ?? !_store.IsToggled(stateVal)));
        }

        public void UpdateSetting(Setting setting, string value)
        {
            switch (setting.Unit)
            {
                case ValueUnit.Degrees:
                case ValueUnit.Integer:
                case ValueUnit.Percentage:
                    if (!Int32.TryParse(value, out var intVal))
                        throw new ArgumentException($"Invalid value for {setting.Name}: {value}");
                    Enqueue(new SettingUpdateCommand(setting, intVal));
                    break;
                default:
                        throw new NotImplementedException();
            }
        }
    }
}