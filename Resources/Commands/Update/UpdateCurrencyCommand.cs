using MediatR;
using trackingApi.Models;

namespace trackingApi.Resources.Commands.Update
{
    public class UpdateCurrencyCommand : IRequest<Currency>
    {
        public int Id { get; set; }
        public string? CurrencyCodeTo { get; set; }
        public string? CurrencyCodeFrom { get; set; }
        public decimal? ConversionRate { get; set; }
    }
}
