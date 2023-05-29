using MediatR;
using trackingApi.Models;
using trackingApi.UnitOfWork;

namespace trackingApi.Resources.Commands.Update
{
    public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, Currency>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCurrencyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Currency> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.GetRepository<Currency>();
            Currency currency = await repository.GetById(request.Id);
            if (currency == null)
            {
                return default;
            }
            currency.CurrencyCodeFrom = request.CurrencyCodeFrom;
            currency.CurrencyCodeTo = request.CurrencyCodeTo;
            currency.ConversionRate = request.ConversionRate;
            //Currency currency = new()
            //{
            //    Id = request.Id,
            //    CurrencyCodeTo = request.CurrencyCodeTo,
            //    CurrencyCodeFrom = request.CurrencyCodeFrom,
            //    ConversionRate = request.ConversionRate
            //};
            await repository.Update(currency);
            return currency;
        }
    }
}
