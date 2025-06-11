namespace EFCoreWebApi.Library
{
    static public class ErrorStatusCodeHandler
    {
        static public Task Handle(StatusCodeContext Context)
        {
            int HttpStatus = Context.HttpContext.Response.StatusCode;

            if (HttpStatus >= 400 && HttpStatus <= 599)
            {
                ApiResult ApiResult = new();
                ApiResult.ErrorResult(HttpStatus);
                string JsonText = JsonSerializer.Serialize(ApiResult);
                Context.HttpContext.Response.WriteAsync(JsonText);
            }

            return Task.CompletedTask;
        }
    }
}
