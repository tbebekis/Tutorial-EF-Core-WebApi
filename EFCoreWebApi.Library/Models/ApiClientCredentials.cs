namespace EFCoreWebApi.Models
{

    /// <summary>
    /// Api Client Credentials request
    /// </summary>
    public class ApiClientCredentials
    {
        /// <summary>
        /// The ClientId
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// The client secret
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// The locale code of the request.
        /// <para>For example <c>en-US</c></para>
        /// <para>SEE: https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims</para>
        /// </summary>
        public string Locale { get; set; }  
    }
}
