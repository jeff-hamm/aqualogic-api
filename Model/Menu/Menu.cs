using System.Collections.Generic;
using AqualogicJumper.Services;

namespace AqualogicJumper.Model
{

    public class Menu : MenuNode
    {
        public Menu()
        {
            Menu = this;
        }

        public override MenuItemType Type => MenuItemType.Menu;
        private IEnumerable<MenuValue> _children;
        public override IEnumerable<MenuValue> Children
        {
            get => _children;
            set
            {
                foreach (var c in value)
                {
                    c.Menu = this;
                }
                _children = value;
            }
        }
    }
}