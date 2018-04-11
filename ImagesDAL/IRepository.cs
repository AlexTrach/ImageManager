using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesDAL
{
    public interface IRepository<T>
    {
        int Add(T entity);
        Task<int> AddAsync(T entity);

        int AddRange(List<T> entities);
        Task<int> AddRangeAsync(List<T> entities);

        int Update(T entity);
        Task<int> UpdateAsync(T entity);

        int Delete(int id);
        Task<int> DeleteAsync(int id);

        int Delete(T entity);
        Task<int> DeleteAsync(T entity);

        T GetOne(int id);
        Task<T> GetOneAsync(int id);

        List<T> GetAll();
        Task<List<T>> GetAllAsync();

        List<T> GetAllEagerly();
        Task<List<T>> GetAllEagerlyAsync();

        List<T> GetSpecifiedRange(int startIndex, int count);
        Task<List<T>> GetSpecifiedRangeAsync(int startIndex, int count);
    }
}
