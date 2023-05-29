using MediatR;
using trackingApi.Data;
using trackingApi.Models;
using trackingApi.UnitOfWork;

namespace trackingApi.Resources.Commands.Create
{
    public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Currency>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateCurrencyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Currency> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currency = new Currency
            {
                CurrencyCodeTo = request.CurrencyCodeTo,
                CurrencyCodeFrom = request.CurrencyCodeFrom,
                ConversionRate = request.ConversionRate,
            };
            var repository = _unitOfWork.GetRepository<Currency>();
            await repository.Create(currency);
            await _unitOfWork.SaveChangesAsync();

            return currency;
        }
    }
}