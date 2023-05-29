
using trackingApi.Models;
namespace trackingApi.GenericRepository
{
    
    
        public interface IGenericRepository<TEntity>
        {
            Task<IEnumerable<TEntity>> GetAll();
            Task<TEntity> GetById(int id);
            Task<TEntity> Create(TEntity entity);
            Task<TEntity> Update(TEntity entity);
            Task<bool> Delete(int id);
        }

    }
