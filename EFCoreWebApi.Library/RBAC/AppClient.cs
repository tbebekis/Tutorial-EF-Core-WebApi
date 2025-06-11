namespace EFCoreWebApi.Entities
{

    /// <summary>
    /// A service or application which is client to this Api 
    /// </summary>
    [Index(nameof(ClientId), IsUnique = true)]
    [Table(nameof(AppClient))]
    public class AppClient: BaseEntity, IAppClient
    {
        List<AppRole> fRoles;
        List<AppPermission> fPermissions;

        public AppClient()
            : base() 
        {
        }
        public AppClient(string ClientId, string PlainTextSecret, string Name = "")
            : this()
        {
            this.SetId();
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
        [Column("Salt"), MaxLength(96), JsonIgnore]
        public string SecretSalt { get; set; }
        /// <summary>
        /// Optional. The requestor name
        /// </summary> 
        public string Name { get; set; }
        /// <summary>
        /// True when requestor is blocked by admins
        /// </summary>
        public bool IsBlocked { get; set; }

        [NotMapped]
        public List<AppRole> Roles
        {
            get
            {
                if (fRoles == null)
                    fRoles = RBAC.GetClientRoles(Id);
                return fRoles;
            }
        }
        [NotMapped]
        public List<AppPermission> Permissions
        {
            get
            {
                if (fPermissions == null)
                    fPermissions = RBAC.GetClientPermissions(Id);
                return fPermissions;
            }
        }
    }

    public class ApiClientEntityTypeConfiguration : IEntityTypeConfiguration<AppClient>
    {
        public void Configure(EntityTypeBuilder<AppClient> builder)
        {
            //builder
            //    .Property(e => e.ClientId)
            //    .IsRequired();
        }
    }
}
