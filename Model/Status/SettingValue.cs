using System;

namespace AqualogicJumper.Model.Status
{
    public class SettingValue : IStatusValue<Setting>
    {
        public Setting Item { get; set; }
        public string Value { get; set; }
        public DateTime? LastModified { get; set; }
        MenuValue IStatusValue.StatusItem
        {
            get => Item;
            set => Item = value as Setting;

        }
        object IStatusValue.Value
        {
            get => Value;
            set => Value = value as string;
        }
    }
}