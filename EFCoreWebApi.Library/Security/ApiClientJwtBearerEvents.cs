namespace EFCoreWebApi.Library
{
    public class ApiClientJwtBearerEvents: JwtBearerEvents
    {

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
            return base.Challenge(context);
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
    }
}
