using System;

namespace AqualogicJumper.Model.Status
{
    public class SensorValue : IStatusValue<Sensor>
    {
        public Sensor Item { get; set; }
        public string Value { get; set; }
        public DateTime? LastModified { get; set; }
        MenuValue IStatusValue.StatusItem
        {
            get => Item;
            set => Item = value as Sensor;

        }
        object IStatusValue.Value
        {
            get => Value;
            set => Value = value as string;
        }
    }
}