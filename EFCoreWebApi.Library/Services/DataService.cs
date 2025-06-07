namespace EFCoreWebApi.Services
{

    /// <summary>
    /// A generic data service for entities derived from <see cref="BaseEntity"/>
    /// </summary>
    public class DataService<T> where T : BaseEntity
    {
   
        /// <summary>
        /// Finds and returns a property by name.
        /// <para>For experimentation only.</para>
        /// </summary>
        protected IProperty FindProperty(string PropertyName)
        {
            return EntityType.FindProperty(PropertyName);
        }

        // ● get entity lists    
        /// <summary>
        /// Returns a list of all entities.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<List<T>> GetAllAsync(AppDbContext DataContext)
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            return await DbSet.AsNoTracking().ToListAsync();
        }    
        /// <summary>
        /// Returns a list of entities passing a <c>raw SQL filter</c>.
        /// <para>A <c>raw SQL filter</c> is the WHERE part of a SQL statement without the <c>WHERE</c> word.</para>
        /// <para>For example: </para>
        /// <para><c>Amount >= 10.5 AND Name like '%John%'</c></para>
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<List<T>> GetByRawSqlFilterAsync(AppDbContext DataContext, string FilterText)
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            string SqlText = $"select * from {TableName} where " + FilterText;
            return await DbSet.FromSqlRaw(SqlText).AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// Returns a list of entities filtered by a specified call-back.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<List<T>> GetByFilterProcAsync(AppDbContext DataContext, Func<T, bool> Proc)
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            return await DbSet.Where(Proc).AsQueryable().AsNoTracking().ToListAsync();
        }

        // ● pagination
        /// <summary>
        /// Returns a list of all entities.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<List<T>> GetPagedAllAsync(AppDbContext DataContext, IPaging Paging)
        {
            List<T> EntityList = await GetAllAsync(DataContext);
            Paging.TotalItems = EntityList.Count;

            return EntityList.Skip(Paging.PageIndex)
                             .Take(Paging.PageSize)
                             .ToList();
        }
        /// <summary>
        /// Returns a list of entities passing a <c>raw SQL filter</c>.
        /// <para>A <c>raw SQL filter</c> is the WHERE part of a SQL statement without the <c>WHERE</c> word.</para>
        /// <para>For example: </para>
        /// <para><c>Amount >= 10.5 AND Name like '%John%'</c></para>
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<List<T>> GetPagedByRawSqlFilterAsync(AppDbContext DataContext, string FilterText, IPaging Paging)
        {
            List<T> EntityList = await GetByRawSqlFilterAsync(DataContext, FilterText);
            Paging.TotalItems = EntityList.Count;

            return EntityList.Skip(Paging.PageIndex)
                             .Take(Paging.PageSize)
                             .ToList();
        }
        /// <summary>
        /// Returns a list of entities filtered by a specified call-back.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<List<T>> GetPagedByFilterProcAsync(AppDbContext DataContext, Func<T, bool> Proc, IPaging Paging)
        {
            List<T> EntityList = await GetByFilterProcAsync(DataContext, Proc);
            Paging.TotalItems = EntityList.Count;

            return EntityList.Skip(Paging.PageIndex)
                             .Take(Paging.PageSize)
                             .ToList();
        }

        // ● get single entity
        /// <summary>
        /// Returns an entity by its Id primary key.
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<T> GetByIdAsync(AppDbContext DataContext, string Id)
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            return await DbSet.FirstOrDefaultAsync(x => x.Id == Id);
        }
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified call-back
        /// </summary>
        protected virtual async Task<T> GetByProcAsync(AppDbContext DataContext, Func<T, bool> Proc)
        {
            await Task.CompletedTask;
            DbSet<T> DbSet = DataContext.Set<T>();
            return DbSet.Where(Proc).FirstOrDefault();
        }

        // ● CRUD
        /// <summary>
        /// Inserts an entity.
        /// <para>Does <strong>not</strong> call <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual T Insert(AppDbContext DataContext, T Entity)
        {
            EntityEntry<T> Entry = DataContext.Add(Entity);
            return Entry.Entity;
        }
        /// <summary>
        /// Updates an entity.
        /// <para>Does <strong>not</strong> call <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual T Update(AppDbContext DataContext, T Entity)
        {
            EntityEntry<T> Entry = DataContext.Update(Entity);
            return Entry.Entity;
        }
        /// <summary>
        /// Deletes an entity.
        /// <para>Does <strong>not</strong> call <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual T Delete(AppDbContext DataContext, T Entity)
        {
            EntityEntry<T> Entry = DataContext.Remove(Entity);
            return Entry.Entity;
        }


        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public DataService()
        {
            using (var DataContext = GetDataContext())
            {
                EntityType = DataContext.Model.FindEntityType(typeof(T));
                string Schema = EntityType.GetSchema();
                TableName = EntityType.GetTableName();
            }
        }

        // ● miscs
        /// <summary>
        /// Creates and returns a <see cref="DbContext"/>.
        /// </summary>
        public virtual AppDbContext GetDataContext()
        {
            AppDbContext Result = Lib.GetDataContext();
            return Result;
        }
        /// <summary>
        /// Executes a specified call-back from insided a transcation.
        /// </summary>
        public virtual void UseTransaction(Action<IDbContextTransaction, AppDbContext> Proc)
        {
            using (var DataContext = GetDataContext())
            {
                using (var Transaction = DataContext.Database.BeginTransaction())
                {
                    Proc(Transaction, DataContext);
                }
            }
        }

        // ● get entity lists
        /// <summary>
        /// Returns all the entities from the database table.
        /// <para>CAUTION: Not all Entities support this call.</para>
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// </summary>
        public virtual async Task<ApiListResult<T>> GetAllAsync()
        {
            ApiListResult<T> Result = new();

            try
            {
                //if (!Bf.In(CRUDMode.GetAll, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: GetAll");

                //BeforeGetAll();

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetAllAsync(DataContext);
                }

                //AfterGetAll(Result);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                //throw;
            }

            return Result;
        }
        /// <summary>
        /// Returns a list of entities passing a <c>raw SQL filter</c>.
        /// <para>A <c>raw SQL filter</c> is the WHERE part of a SQL statement without the <c>WHERE</c> word.</para>
        /// <para>For example: </para>
        /// <para><c>Amount >= 10.5 AND Name like '%John%'</c></para>
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// </summary>
        public virtual async Task<ApiListResult<T>> GetByRawSqlFilterAsync(string FilterText)
        {
            ApiListResult<T> Result = new();

            try
            {
                //if (!Bf.In(CRUDMode.GetAll, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: GetAll");

                //BeforeGetAll();

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetByRawSqlFilterAsync(DataContext, FilterText);
                }

                //AfterGetAll(Result);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                //throw;
            }

            return Result;
        }
        /// <summary>
        /// Returns a list of entities filtered by a specified call-back.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// </summary>
        public virtual async Task<ApiListResult<T>> GetByFilterProcAsync(Func<T, bool> Proc)
        {
            ApiListResult<T> Result = new();

            try
            {
                //if (!Bf.In(CRUDMode.GetAll, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: GetAll");

                //BeforeGetAll();

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetByFilterProcAsync(DataContext, Proc);
                }

                //AfterGetAll(Result);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                //throw;
            }

            return Result;
        }

        // ● pagination
        /// <summary>
        /// Returns a list of all entities.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        public virtual async Task<ApiPagedListResult<T>> GetPagedAllAsync(int PageIndex, int PageSize)
        {
            ApiPagedListResult<T> Result = new();
            try
            {
                Result.PageIndex = PageIndex;
                Result.PageSize = PageSize;

                //if (!Bf.In(CRUDMode.Insert, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: Insert");

                //BeforeGetAll();

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetPagedAllAsync(DataContext, Result);
                }

                //AfterGetAll(Result);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                throw;
            }

            return Result;
        }
        /// <summary>
        /// Returns a list of entities passing a <c>raw SQL filter</c>.
        /// <para>A <c>raw SQL filter</c> is the WHERE part of a SQL statement without the <c>WHERE</c> word.</para>
        /// <para>For example: </para>
        /// <para><c>Amount >= 10.5 AND Name like '%John%'</c></para>
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        public virtual async Task<ApiPagedListResult<T>> GetPagedByRawSqlFilterAsync(string FilterText, int PageIndex, int PageSize)
        {
            ApiPagedListResult<T> Result = new();
            try
            {
                Result.PageIndex = PageIndex;
                Result.PageSize = PageSize;

                //if (!Bf.In(CRUDMode.Insert, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: Insert");

                //BeforeGetAll();

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetPagedByRawSqlFilterAsync(DataContext, FilterText, Result);
                }

                //AfterGetAll(Result);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                throw;
            }

            return Result;
        }
        /// <summary>
        /// Returns a list of entities filtered by a specified call-back.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong> Treat the returned entities as <strong>read-only</strong>.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        public virtual async Task<ApiPagedListResult<T>> GetPagedByFilterProcAsync(Func<T, bool> Proc, int PageIndex, int PageSize)
        {
            ApiPagedListResult<T> Result = new();
            try
            {
                Result.PageIndex = PageIndex;
                Result.PageSize = PageSize;

                //if (!Bf.In(CRUDMode.Insert, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: Insert");

                //BeforeGetAll();

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetPagedByFilterProcAsync(DataContext, Proc, Result);
                }

                //AfterGetAll(Result);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                throw;
            }

            return Result;
        }

        // ● get single entity
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified primary key.
        /// </summary>
        public virtual async Task<ApiItemResult<T>> GetByIdAsync(string Id)
        {
            ApiItemResult<T> Result = new();

            try
            {
                //if (!Bf.In(CRUDMode.GetById, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: GetById");

                //if (Descriptor.PrimaryKeyList.Count > 1)
                //    Sys.Throw($"{EntityType.Name} Entity has a compound primary key");

                //BeforeGetById(Id);

                //var Params = new DynamicParameters();
                //Params.Add(Descriptor.PrimaryKeyList[0].FieldName, Id);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = await GetByIdAsync(DataContext, Id);
                }

                //AfterGetById(Result);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                //throw;
            }

            return Result;
        }
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified call-back
        /// </summary>
        public virtual async Task<ApiItemResult<T>> GetByProcAsync(Func<T, bool> Proc)
        {
            ApiItemResult<T> Result = new();

            try
            {
                //if (!Bf.In(CRUDMode.GetById, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: GetById");

                //if (Descriptor.PrimaryKeyList.Count > 1)
                //    Sys.Throw($"{EntityType.Name} Entity has a compound primary key");

                //BeforeGetById(Id);

                //var Params = new DynamicParameters();
                //Params.Add(Descriptor.PrimaryKeyList[0].FieldName, Id);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = await GetByProcAsync(DataContext, Proc);
                }

                //AfterGetById(Result);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                //throw;
            }

            return Result;
        }

        // ● CRUD
        /// <summary>
        /// Inserts an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ApiItemResult<T>> InsertAsync(T Entity)
        {
            ApiItemResult<T> Result = new();
            try
            {
                //if (!Bf.In(CRUDMode.Insert, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: Insert");

                //BeforeInsert(Entity);
                using (var DataContext = GetDataContext())
                {
                    Result.Item = Insert(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                } 

                //AfterInsert(Entity);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                throw;
            }

            return Result;
        }
        /// <summary>
        /// Updates an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ApiItemResult<T>> UpdateAsync(T Entity)
        {
            ApiItemResult<T> Result = new();
            try
            {
                //if (!Bf.In(CRUDMode.Insert, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: Insert");

                //BeforeInsert(Entity);
                using (var DataContext = GetDataContext())
                {
                    Result.Item = Update(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                }

                //AfterInsert(Entity);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                throw;
            }

            return Result;
        }
        /// <summary>
        /// Deletes an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ApiItemResult<T>> DeleteAsync(T Entity)
        {
            ApiItemResult<T> Result = new();
            try
            {
                //if (!Bf.In(CRUDMode.Insert, Descriptor.Mode))
                //    throw new NotSupportedException($"{EntityType.Name}. CRUD mode not supported: Insert");

                //BeforeInsert(Entity);
                using (var DataContext = L.GetDataContext())
                {
                    Result.Item = Delete(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                }

                //AfterInsert(Entity);
            }
            catch (Exception ex)
            {
                //Sys.LogError(ex, EntityType.Name);
                throw;
            }

            return Result;
        }

        // ● properties
        /// <summary>
        /// Returns the <see cref="IEntityType"/> entity type
        /// </summary>
        public IEntityType EntityType { get; }
        /// <summary>
        /// Returns the table name of the entity.
        /// </summary>
        public string TableName { get; }

 

    }
}
