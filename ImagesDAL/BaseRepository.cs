using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ImagesDAL
{
    public abstract class BaseRepository<T> : IRepository<T>, IDisposable where T: class
    {
        public ImagesContext Context { get; } = new ImagesContext();
        protected DbSet<T> Table { get; set; }

        public int Add(T entity)
        {
            Table.Add(entity);
            return SaveChanges();
        }

        public Task<int> AddAsync(T entity)
        {
            Table.Add(entity);
            return SaveChangesAsync();
        }

        public int AddRange(List<T> entities)
        {
            Table.AddRange(entities);
            return SaveChanges();
        }

        public Task<int> AddRangeAsync(List<T> entities)
        {
            Table.AddRange(entities);
            return SaveChangesAsync();
        }

        public int Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return SaveChanges();
        }

        public Task<int> UpdateAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return SaveChangesAsync();
        }

        public abstract int Delete(int id);

        public abstract Task<int> DeleteAsync(int id);

        public int Delete(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        public T GetOne(int id)
        {
            return Table.Find(id);
        }

        public Task<T> GetOneAsync(int id)
        {
            return Table.FindAsync(id);
        }

        public List<T> GetAll()
        {
            return Table.ToList();
        }

        public Task<List<T>> GetAllAsync()
        {
            return Table.ToListAsync();
        }

        public abstract List<T> GetAllEagerly();

        public abstract Task<List<T>> GetAllEagerlyAsync();

        public abstract List<T> GetSpecifiedRange(int startIndex, int count);

        public abstract Task<List<T>> GetSpecifiedRangeAsync(int startIndex, int count);
        

        protected int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected async Task<int> SaveChangesAsync()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool _disposed = false;

        public void Dispose()
        {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            _disposed = true;
        }

        ~BaseRepository()
        {
            CleanUp(false);
        }
    }
}
