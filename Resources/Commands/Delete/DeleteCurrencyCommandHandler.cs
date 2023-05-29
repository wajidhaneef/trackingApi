using MediatR;
using trackingApi.Data;
using trackingApi.Models;
using trackingApi.UnitOfWork;

namespace trackingApi.Resources.Commands.Delete
{
    public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, Currency>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCurrencyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Currency> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
        {
            //var product = _dbContext.Products.FirstOrDefault(p => p.Id == request.Id);

            //if (product is null)
            //    return default;

            //_dbContext.Remove(product);
            //await _dbContext.SaveChangesAsync();
            var repository = _unitOfWork.GetRepository<Currency>();
            var currency = await repository.GetById(request.Id);
            if (currency == null)
            {
                return default;
            }
            await repository.Delete(request.Id);
            _unitOfWork.SaveChangesAsync();
            return currency;
        }
    }
}