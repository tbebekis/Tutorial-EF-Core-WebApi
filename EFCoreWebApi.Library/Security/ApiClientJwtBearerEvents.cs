using System.Net.Http.Headers;

namespace EFCoreWebApi.Library
{
    public class ApiClientJwtBearerEvents: JwtBearerEvents
    {
        /// <summary>
        /// Invoked if exceptions are thrown during request processing. 
        /// The exceptions will be re-thrown after this event unless suppressed.
        /// </summary>
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        { 
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                var ExpiredException = context.Exception as SecurityTokenExpiredException;

                string DT = JsonSerializer.Serialize(ExpiredException.Expires); 

                // add a custom header
                context.Response.Headers["X-Token-Expired"] = DT;
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// Invoked if Authorization fails and results in a Forbidden response
        /// </summary>
        public override Task Forbidden(ForbiddenContext context)
        {
            return base.Forbidden(context);
        }
        /// <summary>
        /// The <see cref="JwtBearerEvents.MessageReceived"/> event gives the application an opportunity 
        /// to get the token from a different location, adjust, or reject the token.
        /// <para>The application may set the Context.Token in the OnMessageReceived. Otherwise Context.Token is null.</para>
        /// <para>SEE: https://stackoverflow.com/a/54497616/1779320        </para>
        /// </summary>
        public override Task MessageReceived(MessageReceivedContext context)
        {
            return base.MessageReceived(context);
        }
        /// <summary>
        /// The <see cref="JwtBearerEvents.TokenValidated"/> is called 
        /// after the passed in <see cref="TokenValidatedContext.SecurityToken"/> is loaded and validated successfully.
        /// </summary>
        public override Task TokenValidated(TokenValidatedContext context)
        {
            return base.TokenValidated(context);
        }
        /// <summary>
        /// Invoked before a challenge is sent back to the caller.
        /// </summary>
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            ApiResult ApiResult = new();

            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            // error string is a must
            if (string.IsNullOrWhiteSpace(context.Error))
                context.Error = "invalid_token";

            // error description string is a must
            if (string.IsNullOrWhiteSpace(context.ErrorDescription))
                context.ErrorDescription = "Invalid Token. A valid JTW access token is required.";

            ApiResult.NotAuthenticated();

            // Add some extra context for expired tokens.
            if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
            {
                var ExpiredException = context.AuthenticateFailure as SecurityTokenExpiredException;

                string DT = JsonSerializer.Serialize(ExpiredException.Expires);
                string Description = $"The Access Token is expired on {DT}";

                context.ErrorDescription = Description;

                // add a custom header
                context.Response.Headers["X-Token-Expired"] = DT;

                string ErrorMessage = $"{Description}: {context.AuthenticateFailure.Message}";                 
            }

            string JsonText = JsonSerializer.Serialize(ApiResult);
            return context.Response.WriteAsync(JsonText);
        }
    }
}
