namespace EFCoreWebApi.Library
{
    static public partial class DemoData
    {
        static public List<AppClient> GetApiClientList()
        {
            List<AppClient> List = new List<AppClient> 
            {
                new AppClient("client0", "secret", "Client-0"),
                new AppClient("client1", "secret", "Client-1"),
                new AppClient("Admin", "secret", "Admin-0")
            };

            return List;
        }
        static public List<AppPermission> GetAppPermissionList()
        {
            List<AppPermission> List = new List<AppPermission> 
            {
                new AppPermission(AppPermission.RbacAdmin)
            };

            return List;
        }
        static public List<AppRole> GetAppRoleList()
        {
            List<AppRole> List = new List<AppRole> 
            {
                new AppRole("Admin"  ),
                new AppRole("Manager"),
                new AppRole("User"   ),
            };

            return List;
        }

        static public List<AppRolePermission> GetRolePermissionList(List<AppRole> Roles, List<AppPermission> Permissions)
        {
            List<AppRolePermission> List = new List<AppRolePermission> 
            {
                new AppRolePermission(
                    Roles.FirstOrDefault(r => r.Name == "Admin").Id,
                    Permissions.FirstOrDefault(r => r.Name == AppPermission.RbacAdmin).Id
                    )  
            };

            return List;           
        }
        static public List<AppClientRole> GetClientRoleList(List<AppClient> Clients, List<AppRole> Roles)
        {
            //AppClient Client = Clients.FirstOrDefault(r => r.Name == "Admin-0");
            //AppRole Role = Roles.FirstOrDefault(r => r.Name == "Admin");

            List<AppClientRole> List = new List<AppClientRole>()
            {
               new AppClientRole(
                    Clients.FirstOrDefault(r => r.Name == "Admin-0").Id,
                    Roles.FirstOrDefault(r => r.Name == "Admin").Id
                    )
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
            List<AppClient> Clients = GetApiClientList();
            List<AppRole> Roles = GetAppRoleList();
            List<AppPermission> Permissions = GetAppPermissionList();

            List<AppClientRole> ClientRoles = GetClientRoleList(Clients, Roles);
            List<AppRolePermission> RolePermissions = GetRolePermissionList(Roles, Permissions);
            

            List<Product> Products = GetProductList();

            using (var DataContext = new AppDbContext())
            {
                // ● RBAC
                DbSet<AppClient> ClientSet = DataContext.Set<AppClient>();
                ClientSet.AddRange(Clients);

                DbSet<AppRole> RoleSet = DataContext.Set<AppRole>();
                RoleSet.AddRange(Roles);

                DbSet<AppPermission> PermissionSet = DataContext.Set<AppPermission>();
                PermissionSet.AddRange(Permissions);

                DbSet<AppClientRole> ClientRoleSet = DataContext.Set<AppClientRole>();
                ClientRoleSet.AddRange(ClientRoles);

                DbSet<AppRolePermission> RolePermissionSet = DataContext.Set<AppRolePermission>();
                RolePermissionSet.AddRange(RolePermissions);


                // ● Products
                DbSet<Product> ProductSet = DataContext.Set<Product>();
                ProductSet.AddRange(Products);

                DataContext.SaveChanges();
            }
        }
    
        static public void AddData(ModelBuilder modelBuilder)
        {
            List<AppClient> Clients = GetApiClientList();
            List<AppRole> Roles = GetAppRoleList();
            List<AppPermission> Permissions = GetAppPermissionList();

            List<AppClientRole> ClientRoles = GetClientRoleList(Clients, Roles);
            List<AppRolePermission> RolePermissions = GetRolePermissionList(Roles, Permissions);

            List<Product> Products = GetProductList();


            modelBuilder.Entity<AppClient>().HasData(Clients);
            modelBuilder.Entity<AppRole>().HasData(Roles);
            modelBuilder.Entity<AppPermission>().HasData(Permissions);

            modelBuilder.Entity<AppClientRole>().HasData(ClientRoles);
            modelBuilder.Entity<AppRolePermission>().HasData(RolePermissions);

            modelBuilder.Entity<Product>().HasData(Products);
        }
    }
}
