namespace EFCoreWebApi.Entities
{
    [Table(nameof(AppClientRole))]
    [Index(nameof(ClientId), nameof(RoleId), IsUnique = true)]
    public class AppClientRole : BaseEntity
    {
 
        public AppClientRole() 
        { 
        }
        public AppClientRole(string ClientId, string RoleId)
        {
            SetId();
            this.ClientId = ClientId;
            this.RoleId = RoleId;
        }
        
        [Required, MaxLength(40)]
        public string ClientId { get; set; }
        [Required, MaxLength(40)]
        public string RoleId { get; set; }
    }

    public class AppUserRoleTypeConfiguration : IEntityTypeConfiguration<AppClientRole>
    {
        public void Configure(EntityTypeBuilder<AppClientRole> builder)
        {
            //builder
            //    .Property(e => e.ClientId)
            //    .IsRequired();

        }
    }
}
