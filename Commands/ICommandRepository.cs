using trackingApi.Models;

namespace trackingApi.GenericRepository.Commands
{
    public interface ICommandRepository<TEntity>
    {
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task<bool> Delete(int id);
    }
}
