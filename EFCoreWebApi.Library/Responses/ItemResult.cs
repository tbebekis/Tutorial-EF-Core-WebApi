namespace EFCoreWebApi.Responses
{
    /// <summary>
    /// A <see cref="ApiResult"/> response for a single item
    /// </summary>
    [Description("A requested object.")]
    public class ItemResult<T> : ApiResult
    {
        /// <summary>
        /// The item
        /// </summary>
        [Description("The result object.")]
        public T Item { get; set; }
    }
 
}
