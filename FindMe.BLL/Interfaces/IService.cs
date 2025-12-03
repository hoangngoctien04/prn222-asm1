using FindMe.DAL.Models;

namespace FindMe.BLL.Interfaces
{
    public interface IService<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> Find(Func<T, bool> predicate);
    }
}