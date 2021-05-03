namespace AqualogicJumper.Model
{
    public static class MenuItemExtensions
    {
        //
        public static Menu GetMenu(this MenuNode item) =>
                item switch
                {
                    Menu m => m,
                    Sensor s => s.Menu,
                    Switch s => null,
                    _ => null
                };
    }

}