using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AqualogicJumper.Services;

namespace AqualogicJumper.Model
{

    public class MenuMap 
    {
        public IEnumerable<Menu> Menus { get; set; }
        public IEnumerable<Switch> Switches { get; set; }

    }

}
