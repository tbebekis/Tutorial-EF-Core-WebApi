namespace EFCoreWebApi.Entities
{
    public class BaseEntity
    {
        public BaseEntity() 
        { 
            this.Id = Lib.GenId();
        }
        // ● properties
        /// <summary>
        /// Required. 
        /// <para><strong>Unique.</strong></para>
        /// <para>Database Id or something similar.</para>
        /// </summary>
        public string Id { get; set; }
    }
}
