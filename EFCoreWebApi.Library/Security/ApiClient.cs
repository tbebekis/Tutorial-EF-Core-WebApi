namespace EFCoreWebApi.Library
{

    /// <summary>
    /// A service or application which is client to this Api 
    /// </summary>
    public class ApiClient
    {
        // ● properties
        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// <para>Database Id or something similar.</para>
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// </summary> 
        public string ClientId { get; set; }
        /// <summary>
        /// Optional. The requestor name
        /// </summary> 
        public string Name { get; set; }
        /// <summary>
        /// True when requestor is blocked by admins
        /// </summary>
        public bool IsBlocked { get; set; }
    }
}
