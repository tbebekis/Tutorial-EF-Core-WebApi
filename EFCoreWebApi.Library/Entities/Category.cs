namespace EFCoreWebApi.Entities
{
    [Table(nameof(ProductCategory))]
    public class Category: BaseEntity
    {
        public Category()
        {
        }

        public string ParentId { get; set; }
        public string Name { get; set; }
    }
}
