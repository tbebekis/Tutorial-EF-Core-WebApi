namespace EFCoreWebApi.Responses
{
    /// <summary>
    /// Api Token Data
    /// </summary>
    [Description("The ApiToken object.")]
    public class TokenResult
    {
        /// <summary>
        /// The access token value.
        /// </summary>
        [Description("The token string.")]
        public string Token { get; set; }
        /// <summary>
        /// UTC Expiration date and time.
        /// </summary>
        [Description("UTC Expiration date and time.")]
        public DateTime ExpiresOn { get; set; }
    }
}
