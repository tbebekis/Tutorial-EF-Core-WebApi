namespace EFCoreWebApi.Library
{
    public class AppDbContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=EFCoreWebApi.db3", SqliteOptionsBuilder => {
            
            });
        }
    }
}
