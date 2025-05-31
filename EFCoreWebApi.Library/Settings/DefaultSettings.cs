namespace EFCoreWebApi.Library
{

    /// <summary>
    /// Default settings
    /// </summary>
    public class DefaultSettings
    {
        int fCacheTimeoutMinutes = 5;
        int fPageSize = 10;

        /// <summary>
        /// The eviction timeout of an entry from the cache, in minutes. 
        /// <para>Defaults to 0 which means "use the timeouts of the internal implementation".</para>
        /// </summary>
        public int CacheTimeoutMinutes
        {
            get => fCacheTimeoutMinutes >= 5 ? fCacheTimeoutMinutes : 5;
            set => fCacheTimeoutMinutes = value;
        }
        /// <summary>
        /// The number of items in a page when paging is involved.
        /// </summary>
        public int PageSize
        {
            get => fPageSize >= 10 ? fPageSize : 10;
            set => fPageSize = value;
        }
    }
}
