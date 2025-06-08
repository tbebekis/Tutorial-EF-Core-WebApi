namespace EFCoreWebApi.Library
{
    public class AppDbContext: DbContext
    {
        public const string SMemoryDatabase = "MemoryDatabase";
 
        void AddProducts(ModelBuilder modelBuilder)
        {


            //modelBuilder.Entity<Product>()
            //    .HasData(List);

            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        }

        // ● overrides
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Lib.UseInMemoryDatabase)
                optionsBuilder.UseInMemoryDatabase(AppDbContext.SMemoryDatabase);
            else
                optionsBuilder.UseSqlite("Data Source=EFCoreWebApi.db3", SqliteOptionsBuilder => { });
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
