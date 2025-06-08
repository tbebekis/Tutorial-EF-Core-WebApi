namespace EFCoreWebApi.Entities
{
    [Table("Product")]
    [Index(nameof(Name), IsUnique = true)]
    public class Product: BaseEntity
    {
        public Product(string Name, decimal Price)
        {
            SetId();
            this.Name = Name;
            this.Price = Price;
        }
        public override string ToString()
        {
            return Name;
        }

        [Description("Product name.")]
        [Required, MaxLength(256)]
        public string Name { get; set; }
        [Description("Product price.")]
        [Required, Precision(18, 4)]
        public decimal Price { get; set; }
    }
}
