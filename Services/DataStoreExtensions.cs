using System;
using JsonFlatFileDataStore;

namespace AqualogicJumper.Services
{
    public static class DataStoreExtensions
    {
        public static TStore Refresh<TStore>(this DataStore @this, string key, TStore existing=null) where TStore: class,new()
        {
            TStore stored = null;
            try
            {
                if (@this.GetItem(key) != null)
                    stored = @this.GetItem<TStore>(key);
            }
            catch (Exception)
            {
//                _log.LogError(ex, "Error getting status");
            }
            if (stored == null)
            {
                if (existing == null)
                    return new TStore();

                return existing;
            }

            return stored;
        }
    }
}