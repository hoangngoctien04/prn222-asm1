using FindMe.BLL.Interfaces;
using FindMe.DAL.Interfaces;
using FindMe.DAL.Models;

namespace FindMe.BLL.Services
{
    public class Service<T> : IService<T> where T : class
    {
        protected readonly IRepository<T> _repository;

        public Service(IRepository<T> repository)
        {
            _repository = repository;
        }

        public IEnumerable<T> GetAll()
        {
            return _repository.GetAll();
        }

        public T GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Add(T entity)
        {
            _repository.Add(entity);
        }

        public void Update(T entity)
        {
            _repository.Update(entity);
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _repository.Find(x => predicate(x));
        }
    }
}