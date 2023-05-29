using trackingApi.Models;

namespace trackingApi.GenericRepository.Queries
{
    public interface IQueryRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
    }
}
