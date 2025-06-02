namespace EFCoreWebApi
{
    /// <summary>
    /// A <see cref="ApiResponse"/> response for a single item
    /// </summary>
    public class ApiItemResponse<T> : ApiResponse
    {
        /// <summary>
        /// The item
        /// </summary>
        public T Item { get; set; }
    }

    // https://rimdev.io/documenting-aspnetcore-apis-with-swagger
}
