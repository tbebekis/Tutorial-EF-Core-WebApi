namespace EFCoreWebApi.Library
{
    static public class RBAC
    {
        /// <summary>
        /// Creates and returns a <see cref="DbContext"/>.
        /// </summary>
        static AppDbContext GetDataContext()
        {
            AppDbContext Result = Lib.GetDataContext();
            return Result;
        }

        static RBAC()
        {
        }

        static public List<AppRole> GetClientRoles(string Id)
        {
            List<AppRole> Result = new List<AppRole>();

            using (AppDbContext DataContext = GetDataContext())
            {
                DbSet<AppClient> Clients = DataContext.Set<AppClient>();
                DbSet<AppRole> Roles = DataContext.Set<AppRole>();
                DbSet<AppClientRole> ClientRoles = DataContext.Set<AppClientRole>();

                AppClient User = Clients.FirstOrDefault(x => x.Id == Id);
                if (User != null)
                {
                    // get the Ids of the roles the specified client is member of
                    List<string> RoleIdList = ClientRoles.Where(r => r.ClientId == Id)
                                              .Select(x => x.RoleId)
                                              .ToList();

                    // get the role object for each role Id
                    Result = Roles.Where(r => RoleIdList.Contains(r.Id))
                                  .Select(r => r)
                                  .ToList();
                }
            }

            return Result;
        }

        static public List<AppPermission> GetClientPermissions(string Id)
        {
            List<AppPermission> Result = new List<AppPermission>();

            // get the roles the specified client is member of
            List<AppRole> ClientRoleList = GetClientRoles(Id);

            if (ClientRoleList.Count > 0)
            {
                using (AppDbContext DataContext = GetDataContext())
                {
                    DbSet<AppRolePermission> RolePermissions = DataContext.Set<AppRolePermission>();
                    DbSet<AppPermission> Permissions = DataContext.Set<AppPermission>();

                    // for each role the client is member of
                    foreach (var Role in ClientRoleList)
                    {
                        // get the Ids of the permissions of that role in a string list
                        List<string> PermissionIdList = RolePermissions
                                            .Where(p => p.RoleId == Role.Id)
                                            .Select(x => x.PermissionId)
                                            .ToList();

                        if (PermissionIdList.Count > 0)
                        {
                            // get the permission object for each permission Id
                            List<AppPermission> List = Permissions.Where(p => PermissionIdList.Contains(p.Id))
                                                .Select(p => p)
                                                .ToList();

                            // add the permission object to the result list, if not already there
                            foreach (var Permission in List)
                            {
                                if (Result.FirstOrDefault(x => x.Id == Permission.Id) == null)
                                    Result.Add(Permission);
                            }
                        }
                    }
                }
            }



            return Result;
        }
    }
}
