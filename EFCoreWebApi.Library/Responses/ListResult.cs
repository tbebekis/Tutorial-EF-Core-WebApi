namespace EFCoreWebApi.Responses
{
    /// <summary>
    /// A <see cref="ApiResult"/> response for lists of items.
    /// </summary>
    [Description("A list of requested objects.")]
    public class ListResult<T> : ApiResult
    {
        /// <summary>
        /// The list of items
        /// </summary>
        [Description("A list of result objects."), JsonPropertyOrder(-901)]
        public List<T> List { get; set; } = new List<T>();
    }
}
