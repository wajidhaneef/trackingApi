using trackingApi.Models;
namespace trackingApi.NonGenericRepository
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetNewsByTitle(string title);
    }
}
