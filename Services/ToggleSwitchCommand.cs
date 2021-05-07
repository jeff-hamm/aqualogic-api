using System;

namespace AqualogicJumper.Services
{
    public class ToggleSwitchCommand : AqualogicCommand
    {
        public SwitchName Switch { get; }
        public bool Value { get; }
        public override int Priority { get; } = 2;

        public ToggleSwitchCommand(SwitchName @switch, bool value)
        {
            Switch = @switch;
            Value = value;
        }
        protected override bool IsComplete(PoolStatusStore state)
        {
            var current = state.ToggledSwitches.Contains(Switch);
            return (current == Value);
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