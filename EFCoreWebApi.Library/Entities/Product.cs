namespace EFCoreWebApi.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Product: BaseEntity
    {
        public Product(string Name, decimal Price)
        {
            this.Name = Name;
            this.Price = Price;
        }
        public override string ToString()
        {
            return Name;
        }

        [Required, MaxLength(256)]
        public string Name { get; set; }
        [Required, Precision(18, 4)]
        public decimal Price { get; set; }
    }
}
