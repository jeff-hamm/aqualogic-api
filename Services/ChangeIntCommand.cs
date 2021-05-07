using System;
using AqualogicJumper.Model;

namespace AqualogicJumper.Services
{
    public class SettingUpdateCommand : MenuSelectCommand
    {
        private readonly Setting _setting;
        public int Value { get; }

        public SettingUpdateCommand(Setting setting, int value) : base(setting)
        {
            _setting = setting;
            Value = value;
        }

        protected override bool IsComplete(PoolStatusStore state) =>
            base.IsComplete(state) && CurrentValue(state) == Value;

        private int? CurrentValue(PoolStatusStore state) =>
            state.Status.Settings.TryGetValue(_setting.Name, out var value) &&
                               Int32.TryParse(value.Value, out var intVal) ? intVal : null;

        protected override Key NextKey(PoolStatusStore state)
        {
            if (!base.IsComplete(state))
                return base.NextKey(state);
            var value = CurrentValue(state);
            if (!value.HasValue) return Key.PLUS;
            return Value > value ? Key.PLUS : Key.MINUS;
        }

        public override string ToString() => $"SettingUpdate {_setting.Name}: {Value}";
    }
}