namespace EFCoreWebApi.Entities
{

    /// <summary>
    /// A service or application which is client to this Api 
    /// </summary>
    public class ApiClient: BaseEntity, IApiClient
    {
        public ApiClient()
            : base() 
        {
        }
        public ApiClient(string ClientId, string PlainTextSecret, string Name = "")
            : this()
        { 
            this.ClientId = ClientId;
            this.SecretSalt = Hasher.GenerateSalt(96);
            this.Secret = Hasher.Hash(PlainTextSecret, this.SecretSalt);
            this.Name = Name;
        }

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Name)? Name: ClientId;
        }


        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// </summary> 
        [Required, MaxLength(96)]
        public string ClientId { get; set; }
        /// <summary>
        /// The client secret encrypted
        /// <para><strong>Encrypted.</strong></para>
        /// </summary>
        [MaxLength(64)]
        public string Secret { get; set; }
        /// <summary>
        /// The client secret salt
        /// </summary>
        [Column("Salt"), MaxLength(96)]
        public string SecretSalt { get; set; }
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
