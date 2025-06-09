namespace EFCoreWebApi.Services
{

    /// <summary>
    /// A generic data service for entities derived from <see cref="BaseEntity"/>
    /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
    /// <para>NOTE: This service should be used with <strong>Disconnected Entities</strong>, SEE: https://learn.microsoft.com/en-us/ef/core/saving/disconnected-entities </para>
    /// </summary>
    public class DataService<T> where T : BaseEntity
    {
        string fTableName;

        protected virtual void CheckCRUDMode(CRUDMode Mode)
        {
            bool IsSet = (AllowedCRUDModes & Mode) == Mode;
            if (!IsSet)
                throw new ApplicationException($"CRUD mode not supported: {Mode}");
        }

        // ● get entity lists    
        /// <summary>
        /// Returns a list of all entities.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
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
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
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
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<List<T>> GetByFilterProcAsync(AppDbContext DataContext, Func<T, bool> Proc)
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            return await DbSet.AsNoTracking().Where(Proc).AsQueryable().ToListAsync();
        }

        // ● pagination
        /// <summary>
        /// Returns a list of all entities.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
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
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
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
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
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
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<T> GetByIdAsync(AppDbContext DataContext, string Id)
        {
            DbSet<T> DbSet = DataContext.Set<T>();
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
        }
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified call-back
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        protected virtual async Task<T> GetByProcAsync(AppDbContext DataContext, Func<T, bool> Proc)
        {
            DbSet<T> DbSet = DataContext.Set<T>();           
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => Proc(x));    //return DbSet.Where(Proc).FirstOrDefault();
        }

        // ● CRUD
        /// <summary>
        /// Inserts an entity.
        /// <para>Does <strong>not</strong> call <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// <para>NOTE: if the specified entity comes without an Id, a new one Id is assigned to it.</para>
        /// </summary>
        protected virtual T Insert(AppDbContext DataContext, T Entity)
        {
            if (string.IsNullOrWhiteSpace(Entity.Id))
                Entity.SetId();

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
            Type ClassType = typeof(T);
            if (ClassType.IsDefined(typeof(CRUDModeAttribute)))
            {
                var Attr = Attribute.GetCustomAttribute(ClassType, typeof(CRUDModeAttribute), true) as CRUDModeAttribute;
                AllowedCRUDModes = Attr.Modes;
            }
            else
            {
                AllowedCRUDModes = CRUDMode.All;
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
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// </summary>
        public virtual async Task<ListResult<T>> GetAllAsync()
        {
            ListResult<T> Result = new();

            try
            {
                CheckCRUDMode(CRUDMode.GetAll);

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetAllAsync(DataContext);
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Returns a list of entities passing a <c>raw SQL filter</c>.
        /// <para>A <c>raw SQL filter</c> is the WHERE part of a SQL statement without the <c>WHERE</c> word.</para>
        /// <para>For example: </para>
        /// <para><c>Amount >= 10.5 AND Name like '%John%'</c></para>
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// </summary>
        public virtual async Task<ListResult<T>> GetByRawSqlFilterAsync(string FilterText)
        {
            ListResult<T> Result = new();

            try
            {
                CheckCRUDMode(CRUDMode.GetByFilter);

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetByRawSqlFilterAsync(DataContext, FilterText);
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Returns a list of entities filtered by a specified call-back.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// </summary>
        public virtual async Task<ListResult<T>> GetByFilterProcAsync(Func<T, bool> Proc)
        {
            ListResult<T> Result = new();

            try
            {
                CheckCRUDMode(CRUDMode.GetByFilter);

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetByFilterProcAsync(DataContext, Proc);
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }

        // ● pagination
        /// <summary>
        /// Returns a list of all entities.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        public virtual async Task<ListResultPaged<T>> GetPagedAllAsync(int PageIndex, int PageSize)
        {
            ListResultPaged<T> Result = new();
            try
            {
                Result.PageIndex = PageIndex;
                Result.PageSize = PageSize;

                CheckCRUDMode(CRUDMode.GetAll);

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetPagedAllAsync(DataContext, Result);
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Returns a list of entities passing a <c>raw SQL filter</c>.
        /// <para>A <c>raw SQL filter</c> is the WHERE part of a SQL statement without the <c>WHERE</c> word.</para>
        /// <para>For example: </para>
        /// <para><c>Amount >= 10.5 AND Name like '%John%'</c></para>
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        public virtual async Task<ListResultPaged<T>> GetPagedByRawSqlFilterAsync(string FilterText, int PageIndex, int PageSize)
        {
            ListResultPaged<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetByFilter);

                Result.PageIndex = PageIndex;
                Result.PageSize = PageSize;

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetPagedByRawSqlFilterAsync(DataContext, FilterText, Result);
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Returns a list of entities filtered by a specified call-back.
        /// <para>WARNING: <strong>Change tracking is disabled.</strong></para>
        /// <para>NOTE: it can be used from inside a transaction.</para>
        /// </summary>
        public virtual async Task<ListResultPaged<T>> GetPagedByFilterProcAsync(Func<T, bool> Proc, int PageIndex, int PageSize)
        {
            ListResultPaged<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.GetByFilter);

                Result.PageIndex = PageIndex;
                Result.PageSize = PageSize;

                using (var DataContext = GetDataContext())
                {
                    Result.List = await GetPagedByFilterProcAsync(DataContext, Proc, Result);
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }

        // ● get single entity
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified primary key.
        /// </summary>
        public virtual async Task<ItemResult<T>> GetByIdAsync(string Id)
        {
            ItemResult<T> Result = new();

            try
            {
                CheckCRUDMode(CRUDMode.GetById);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = await GetByIdAsync(DataContext, Id);
                }

            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Selects and returns a single entity from the database, based on a specified call-back
        /// </summary>
        public virtual async Task<ItemResult<T>> GetByProcAsync(Func<T, bool> Proc)
        {
            ItemResult<T> Result = new();

            try
            {
                CheckCRUDMode(CRUDMode.GetByFilter);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = await GetByProcAsync(DataContext, Proc);
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }

        // ● CRUD
        /// <summary>
        /// Inserts an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ItemResult<T>> InsertAsync(T Entity)
        {
            ItemResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.Insert);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = Insert(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                } 
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Updates an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ItemResult<T>> UpdateAsync(T Entity)
        {
            ItemResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.Update);
 
                using (var DataContext = GetDataContext())
                {
                    Result.Item = Update(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }
        /// <summary>
        /// Deletes an entity.
        /// <para>Calls <c>SaveChanges()</c>.</para>
        /// <para>Returns the <strong>trackable</strong> entity.</para>
        /// </summary>
        public virtual async Task<ItemResult<T>> DeleteAsync(T Entity)
        {
            ItemResult<T> Result = new();
            try
            {
                CheckCRUDMode(CRUDMode.Delete);

                using (var DataContext = GetDataContext())
                {
                    Result.Item = Delete(DataContext, Entity);
                    await DataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Result.ExceptionResult(ex);
            }

            return Result;
        }

        // ● properties
 
        /// <summary>
        /// Returns the table name of the entity.
        /// </summary>
        public string TableName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fTableName))
                {
                    using (var DataContext = GetDataContext())
                    {
                        DbSet<T> DbSet = DataContext.Set<T>();
                        IEntityType EntityType = DataContext.Model.FindEntityType(typeof(T));
                        if (EntityType != null)
                            fTableName = EntityType.GetTableName();

                        if (string.IsNullOrWhiteSpace(fTableName))
                        {
                            Type ClassType = typeof(T);

                            if (ClassType.IsDefined(typeof(TableAttribute)))
                            {
                                var Attr = Attribute.GetCustomAttribute(ClassType, typeof(TableAttribute), true) as TableAttribute;
                                fTableName = Attr.Name;
                            }

                            if (string.IsNullOrWhiteSpace(fTableName))
                                fTableName = ClassType.Name;
                        }

                    }
                }

                return fTableName;
            }
        }
        /// <summary>
        /// A bit-field indicating the CRUD operations allowed to this Entity
        /// </summary>
        public CRUDMode AllowedCRUDModes { get; }
 

    }
}
