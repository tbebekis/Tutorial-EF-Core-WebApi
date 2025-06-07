namespace EFCoreWebApi.Library
{
    public class AppDbContext: DbContext
    {
        public const string SMemoryDatabase = "MemoryDatabase";
 
        void AddProducts(ModelBuilder modelBuilder)
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

            modelBuilder.Entity<Product>()
                .HasData(List);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddProducts(modelBuilder);
        }

        // ● overrides
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data Source=EFCoreWebApi.db3", SqliteOptionsBuilder => { });
        }

        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public AppDbContext()
            : this(new DbContextOptions<AppDbContext>())
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// If the DbContext subtype is itself intended to be inherited from, then it should expose a protected constructor taking a non-generic DbContextOptions
        /// SEE: https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#dbcontextoptions-versus-dbcontextoptionstcontext
        /// </summary>
        protected AppDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
        {
        }

        // ● Db Sets
        public DbSet<Product> Products { get; set; }
    }
}
