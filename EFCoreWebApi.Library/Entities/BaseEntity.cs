namespace EFCoreWebApi.Entities
{
    public class BaseEntity
    {
        public BaseEntity() 
        { 
        }


        // ● properties
        /// <summary>
        /// Required. 
        /// <para><strong>Primary Key. Unique.</strong></para>
        /// <para>Database Id or something similar.</para>
        /// </summary>
        [Key, MaxLength(40)]
        public string Id { get; set; } = Lib.GenId();
    }
}
