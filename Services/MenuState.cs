using System.Text.RegularExpressions;
using AqualogicJumper.Model;

namespace AqualogicJumper.Services
{
    public class MenuState {
        public MenuState(MenuNode item, Match match, StatusUpdate status)
        {
            Current = item;
            TextMatch = match;
            Text = status;
        }
        public MenuNode Current { get; set; }
        public Menu Menu => Current.Menu;
        public Sensor Sensor => Current as Sensor;
        public Setting Setting => Current as Setting;
        public Match TextMatch { get; }
        public StatusUpdate Text { get; }
    }
}