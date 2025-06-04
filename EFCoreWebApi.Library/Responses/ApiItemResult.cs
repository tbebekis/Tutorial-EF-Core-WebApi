namespace EFCoreWebApi
{
    /// <summary>
    /// A <see cref="ApiResult"/> response for a single item
    /// </summary>
    public class ApiItemResult<T> : ApiResult
    {
        static public ApiItemResult<T> ErrorResult(int HttpStatus, string ErrorMessage)
        {
            ApiItemResult<T> Result = new ApiItemResult<T>();
            Result.HttpStatus = HttpStatus;
            Result.AddError(ErrorMessage);
            return Result;
        }
        static public ApiItemResult<T> BadRequest(string ErrorMessage = "")
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? $"Bad request: {ErrorMessage}" : "Bad request";
            return ErrorResult(StatusCodes.Status400BadRequest, ErrorMessage);
        }
        static public ApiItemResult<T> NoDataResult(string ErrorMessage = "")
        {
            ErrorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? ErrorMessage : "No data.";
            return BadRequest(ErrorMessage);
        }

        /// <summary>
        /// The item
        /// </summary>
        [Description("The result object.")]
        public T Item { get; set; }
    }
 
}
