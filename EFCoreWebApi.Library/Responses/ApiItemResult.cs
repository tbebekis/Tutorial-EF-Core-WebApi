namespace EFCoreWebApi
{
    /// <summary>
    /// A <see cref="ApiResult"/> response for a single item
    /// </summary>
    public class ApiItemResult<T> : ApiResult
    {
 

        /// <summary>
        /// The item
        /// </summary>
        [Description("The result object.")]
        public T Item { get; set; }
    }
 
}
