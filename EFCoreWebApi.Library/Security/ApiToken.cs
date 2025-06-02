namespace EFCoreWebApi.Library
{
    /// <summary>
    /// Api Token Data
    /// </summary>
    public class ApiToken
    {
        /// <summary>
        /// The access token value.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// UTC Expiration date and time.
        /// </summary>
        public string ExpiresOn { get; set; }
    }
}
