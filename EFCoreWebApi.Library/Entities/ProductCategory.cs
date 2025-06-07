namespace EFCoreWebApi.Entities
{
    [Index(nameof(ProductId), nameof(CategoryId), IsUnique = true)]
    public class ProductCategory: BaseEntity
    {
        public ProductCategory()
        {
        }

        public string ProductId { get; set; }
        public string CategoryId { get; set; }
    }
}
