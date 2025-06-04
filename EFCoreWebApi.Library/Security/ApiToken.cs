namespace EFCoreWebApi.Library
{
    /// <summary>
    /// Api Token Data
    /// </summary>
    [Description("The ApiToken object.")]
    public class ApiToken
    {
        /// <summary>
        /// The access token value.
        /// </summary>
        [Description("The token string.")]
        public string Token { get; set; }
        /// <summary>
        /// UTC Expiration date and time.
        /// </summary>
        [Description("UTC Expiration date and time as string.")]
        public string ExpiresOn { get; set; }
    }
}
