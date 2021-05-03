using AqualogicJumper.Services;

namespace AqualogicJumper.Model
{

    public class Switch : MenuValue
    {
        public override MenuItemType Type => MenuItemType.Switch;
        public SwitchName SwitchName { get; set; }
        public bool Toggled { get; set; }
    }
}