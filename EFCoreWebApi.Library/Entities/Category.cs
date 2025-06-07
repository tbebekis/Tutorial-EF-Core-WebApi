namespace EFCoreWebApi.Entities
{
    public class Category: BaseEntity
    {
        public Category()
        {
        }

        public string ParentId { get; set; }
        public string Name { get; set; }
    }
}
