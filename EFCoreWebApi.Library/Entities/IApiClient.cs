namespace EFCoreWebApi.Entities
{
    public interface IApiClient
    {
        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// <para>Database Id or something similar.</para>
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// </summary> 
        string ClientId { get; set; }
        /// <summary>
        /// Optional. The requestor name
        /// </summary> 
        string Name { get; set; }
        /// <summary>
        /// True when requestor is blocked by admins
        /// </summary>
        public bool IsBlocked { get; set; }
    }
}
