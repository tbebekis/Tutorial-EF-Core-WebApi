namespace EFCoreWebApi.Library
{

    /// <summary>
    /// A global exception handler
    /// <para>SEE: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling</para>
    /// <para>SEE: https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors</para>
    /// </summary>
    public class ExceptionHandler : IExceptionHandler
    {
        public ExceptionHandler()
        {
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            ApiResult ApiResult = new();
            ApiResult.ExceptionResult(exception);

            string JsonText = JsonSerializer.Serialize(ApiResult);
            await httpContext.Response.WriteAsync(JsonText);

            return true;
        }
    }
}
