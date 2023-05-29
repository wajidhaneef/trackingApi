using trackingApi.NonGenericRepository;
using trackingApi.Data;
using trackingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace trackingApi.NonGenericRepository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly IssueDbContext _context;
        public CurrencyRepository(IssueDbContext context) => _context = context;

        
        public async Task<IEnumerable<Currency>> GetCurrenciesByCodeFrom(string codeFrom)
        {
            return await _context.Currency.Where(c => c.CurrencyCodeFrom == codeFrom).ToListAsync();
        }
     
        public async Task<IEnumerable<Currency>> GetCurrencyConverter(string codeTo, string codeFrom)
        {
            return await _context.Currency.Where(c => c.CurrencyCodeFrom == codeFrom && c.CurrencyCodeTo==codeTo).ToListAsync();
        }
    }
}
