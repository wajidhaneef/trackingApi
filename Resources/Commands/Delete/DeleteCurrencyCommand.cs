using MediatR;
using trackingApi.Models;

namespace trackingApi.Resources.Commands.Delete
{
    public class DeleteCurrencyCommand : IRequest<Currency>
    {
        public int Id { get; set; }
    }
}
