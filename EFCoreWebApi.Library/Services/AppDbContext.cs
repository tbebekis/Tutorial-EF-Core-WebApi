namespace EFCoreWebApi.Library
{
    public class AppDbContext: DbContext
    {
        public const string SMemoryDatabase = "MemoryDatabase";

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


    }
}
