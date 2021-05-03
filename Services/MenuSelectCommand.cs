using AqualogicJumper.Model;
using System;

namespace AqualogicJumper.Services
{
    public class MenuSelectCommand : AqualogicCommand
    {
        public MenuSelectCommand(MenuNode toSelect)
        {
            if (toSelect == null) throw new ArgumentNullException(nameof(toSelect));
            if (String.IsNullOrEmpty(toSelect?.Menu?.Name)) throw new ArgumentNullException("Menu Name");
            this.ToSelect = toSelect;
        }
        public MenuNode ToSelect { get; }
        protected override bool IsComplete(PoolStatusStore state) =>
            state.CurrentMenu.Current == ToSelect;
        protected override Key NextKey(PoolStatusStore state)
        {
            if (state.CurrentMenu.Current?.Menu?.Name?.Equals(ToSelect.Menu.Name) != true)
                return Key.MENU;
            return Key.RIGHT;
        }

        public override string ToString()
        {
            return $"MenuSelect: {ToSelect?.Name}";
        }
    }
}