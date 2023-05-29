using trackingApi.Models;

namespace trackingApi.NonGenericRepository
{
    public interface IWeatherRepository
    {
        Task<IEnumerable<Weather>> GetWeatherByCity(string city);
        Task<IEnumerable<Weather>> GetWeatherByCountry(string country);
    }
}
