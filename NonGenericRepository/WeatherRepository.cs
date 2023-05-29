using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackingApi.Data;
using trackingApi.Models;

namespace trackingApi.NonGenericRepository
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly IssueDbContext _context;
        public WeatherRepository(IssueDbContext context) => _context = context;

        async public Task<IEnumerable<Weather>> GetWeatherByCity(string city)
        {
            return await _context.Weathers.Where(c => c.City == city).ToListAsync();
        }
        async public Task<IEnumerable<Weather>> GetWeatherByCountry(string country)
        {
            return await _context.Weathers.Where(c => c.Country == country).ToListAsync();
        }
    }
}
