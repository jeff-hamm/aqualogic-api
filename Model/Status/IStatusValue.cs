using System;

namespace AqualogicJumper.Model.Status
{
    public interface IStatusValue
    {
        MenuValue StatusItem { get; set; }
        object Value { get; set; }
        DateTime? LastModified { get; set; }

    }

    public interface IStatusValue<TItem> : IStatusValue where TItem:MenuValue
    {
        TItem Item { get; set; }

    }
}