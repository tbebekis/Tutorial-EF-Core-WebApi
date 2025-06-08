namespace EFCoreWebApi.Entities
{
     
    public class BaseEntity
    {
        public BaseEntity() 
        { 
        }

        public virtual void SetId()
        {
            this.Id = Lib.GenId();
        }

        // ● properties
        /// <summary>
        /// Required. 
        /// <para><strong>Primary Key. Unique.</strong></para>
        /// <para>Database Id or something similar.</para>
        /// </summary>
        [Description("Primary Key. Unique. Id in the database table.")]
        [Key, MaxLength(40), DefaultValue(null), JsonPropertyOrder(-1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
    }
}
