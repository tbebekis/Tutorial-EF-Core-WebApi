namespace EFCoreWebApi.Responses
{
    /// <summary>
    /// A <see cref="ApiResult"/> response for lists of items.
    /// </summary>
    public class ListResult<T> : ApiResult
    {
        /// <summary>
        /// The list of items
        /// </summary>
        [Description("A list of result objects.")]
        public List<T> List { get; set; } = new List<T>();
    }
}
