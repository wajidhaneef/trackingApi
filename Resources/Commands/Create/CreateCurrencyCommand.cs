using MediatR;
using trackingApi.Models;

namespace trackingApi.Resources.Commands.Create
{
    public class CreateCurrencyCommand : IRequest<Currency>
    {
        public string? CurrencyCodeTo { get; set; }
        public string? CurrencyCodeFrom { get; set; }
        public decimal? ConversionRate { get; set; }
    }
}
