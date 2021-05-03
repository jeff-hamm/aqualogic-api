using System;

namespace AqualogicJumper.Services
{
    public class ToggleSwitchCommand : AqualogicCommand
    {
        public SwitchName Switch { get; }
        public bool Value { get; }

        public ToggleSwitchCommand(SwitchName @switch, bool value)
        {
            Switch = @switch;
            Value = value;
        }
        protected override bool IsComplete(PoolStatusStore state)
        {
            var current = state.IsToggled(Switch);
            if (current == Value) return true;
            return !state.PendingSwitches.TryGetValue(Switch, out var p) ||
                                               state.ToggledSwitches.Contains(Switch) == Value;
        }

        protected override Key NextKey(PoolStatusStore state)
        {
            if (!Enum.TryParse(typeof(Key), Switch.ToString(), true, out var k) || !(k is Key key))
                throw new ArgumentException($"Unable to find key corresponding to switch name {Switch}");
            state.PendingSwitches.AddOrUpdate(Switch, n => new SwitchValue()
            {
                Name = Switch,
                Value = Value,
                LastModified = DateTime.UtcNow
            }, (n, v) =>
            {
                v.Value = Value;
                return v;
            });
            return key;

        }

        public override string ToString() => $"ToggleSwitch {Switch}: [{Value}]";
    }
}