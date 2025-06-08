namespace EFCoreWebApi.Library
{
    public class JwtSettings
    {
        /// <summary>
        /// A string used in encrypting and signing the Jwt token.
        /// </summary>
        public string EncryptionKey { get; set; } = "{D87E5DB0-42DE-4314-A140-16512E05CEA4}";
        /// <summary>
        /// Optional. A case-sensitive string or URI value representing the entity that generates the tokens.
        /// <para>SEE: https://datatracker.ietf.org/doc/html/rfc7519#section-4.1.1 </para>
        /// </summary>
        public string Issuer { get; set; } = "EFCoreWebApi";
        /// <summary>
        /// Optional. A string array or a single string or URI value identifying the recipients that the Jwt Token is intended for.
        /// <para>SEE: https://datatracker.ietf.org/doc/html/rfc7519#section-4.1.3 </para>
        /// </summary>
        public string Audience { get; set; } = "All";
        /// <summary>
        /// The number of minutes a Jwt is valid.
        /// </summary>
        public int LifeTimeMinutes { get; set; } = 300;
    }
}
 