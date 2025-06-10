namespace EFCoreWebApi.Library
{
    static public partial class DemoData
    {
        static public List<ApiClient> GetApiClientList()
        {
            List<ApiClient> List = new()
            {
                new ApiClient("client0", "secret", "Client-0"),
                new ApiClient("client1", "secret", "Client-1"),
                new ApiClient("client2", "secret", "Client-2")
            };

            return List;
        }
        static public List<Product> GetProductList()
        {
            Random R = new Random();

            decimal GetPrice()
            {
                decimal Result = Convert.ToDecimal(R.NextDouble());
                Result = Math.Round(Result, 2);
                Result += R.Next(2, 40);
                return Result;
            }

            List<Product> List = new List<Product>()
            {
                new Product("Absorbent cotton", GetPrice()),
                new Product("Alfalfa pellets", GetPrice()),
                new Product("Allspice", GetPrice()),
                new Product("Almonds", GetPrice()),
                new Product("Aniseed", GetPrice()),
                new Product("Apples", GetPrice()),
                new Product("Apples, dried", GetPrice()),
                new Product("Apricot kernels", GetPrice()),
                new Product("Apricots, dried", GetPrice()),
                new Product("Artichokes", GetPrice()),
                new Product("Asparagus", GetPrice()),
                new Product("Automobiles", GetPrice()),
                new Product("Avocados", GetPrice()),
                new Product("Bananas", GetPrice()),
                new Product("Barley", GetPrice()),
                new Product("Bay leaves", GetPrice())
            };

            return List;
        }


        static public void AddInMemoryData()
        {
            List<ApiClient> Clients = GetApiClientList();
            List<Product> Products = GetProductList();

            using (var DataContext = new AppDbContext())
            {
                DbSet<ApiClient> ClientSet = DataContext.Set<ApiClient>();
                ClientSet.AddRange(Clients);

                DbSet<Product> ProductSet = DataContext.Set<Product>();
                ProductSet.AddRange(Products);


                DataContext.SaveChanges();
            }
        }
    }
}
