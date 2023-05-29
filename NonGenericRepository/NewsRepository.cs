using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackingApi.Data;
using trackingApi.Models;

namespace trackingApi.NonGenericRepository
{
    public class NewsRepository : INewsRepository
    {
        private readonly IssueDbContext _context;
        public NewsRepository(IssueDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<News>> GetNewsByTitle(string title)
        {
            return await _context.News.Where(c => c.Title == title).ToListAsync();
        }
    }
}
