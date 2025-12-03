using FindMe.DAL.Interfaces;
using FindMe.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FindMe.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly FindMeDbContext _context;

        public Repository(FindMeDbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }
    }
}