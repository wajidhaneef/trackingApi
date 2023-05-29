using Microsoft.EntityFrameworkCore;
using trackingApi.Data;
using trackingApi.GenericRepository.Queries;

namespace trackingApi.GenericRepository
{
    public class QueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : class
    {
        private readonly IssueDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public QueryRepository(IssueDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetById(int id) => await _dbSet.FindAsync(id);
    }
}
