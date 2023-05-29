using trackingApi.GenericRepository;
using trackingApi.Models;

namespace trackingApi.NonGenericRepository
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<Currency>> GetCurrenciesByCodeFrom(string code);
        Task<IEnumerable<Currency>> GetCurrencyConverter(string codeTo, string codeFrom);
    }
}
