using System;

namespace AqualogicJumper.Model.Status
{
    public class SwitchValue : IStatusValue<Switch>
    {
        public Switch Item { get; set; }
        public bool Value { get; set; }
        public DateTime? LastModified { get; set; }
        MenuValue IStatusValue.StatusItem
        {
            get => Item;
            set => Item = value as Switch;

        }
        object IStatusValue.Value
        {
            get => Value;
            set => Value = value is bool b && b;
        }

    }
}