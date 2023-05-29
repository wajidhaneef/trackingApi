using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using trackingApi.Data;
using trackingApi.Dtos;
using trackingApi.GenericRepository;
using trackingApi.Models;
using trackingApi.NonGenericRepository;
using trackingApi.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;
using trackingApi.OpenAPiService1;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using static System.Net.WebRequestMethods;
using System.Runtime.InteropServices;
using trackingApi.Resources.Commands.Create;
using MediatR;
using Azure.Core;
using trackingApi.Resources.Commands.Delete;

namespace trackingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CurrencyController> _logger;
        private readonly IMediator _mediator;

        public CurrencyController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CurrencyController> logger, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Retrieving currencies...");

            var currencyRepository = _unitOfWork.GetRepository<Currency>();
            var currencies = await currencyRepository.GetAll();
            var currencyDtos = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);

            _logger.LogInformation("Currencies retrieved successfully.");

            return Ok(currencyDtos);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Retrieving currency by ID...");

            var currencyRepository = _unitOfWork.GetRepository<Currency>();
            var currency = await currencyRepository.GetById(id);
            if (currency == null)
            {
                _logger.LogWarning($"Currency with ID {id} not found.");
                return NotFound();
            }

            var currencyDto = _mapper.Map<CurrencyDto>(currency);

            _logger.LogInformation($"Currency with ID {id} retrieved successfully.");
            return Ok(currencyDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CurrencyDto currencyDto)
        {
            _logger.LogInformation("Creating a new currency...");

            if (currencyDto == null)
            {
                _logger.LogWarning("Invalid currency data provided.");
                return BadRequest();
            }
            /*try
            {
                var currency = _mapper.Map<Currency>(currencyDto);
                var currencyRepository = _unitOfWork.GetRepository<Currency>();
                _unitOfWork.CreateTransaction();
                await currencyRepository.Create(currency);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                var createdCurrencyDto = _mapper.Map<CurrencyDto>(currency);

                _logger.LogInformation($"Currency with ID {currency.Id} created successfully.");

                return CreatedAtAction(nameof(GetById), new { id = currency.Id }, createdCurrencyDto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                _unitOfWork.Rollback();

                return BadRequest(ex.Message);
            }*/
            try
            {
                var command = new CreateCurrencyCommand()
                {
                    CurrencyCodeTo = currencyDto.CurrencyCodeTo,
                    CurrencyCodeFrom = currencyDto.CurrencyCodeFrom,
                    ConversionRate = currencyDto.ConversionRate,
                };
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(CurrencyDto currencyDto, int id)
        {
            _logger.LogInformation($"Updating currency with ID {id}...");

            if (currencyDto == null)
            {
                _logger.LogWarning("Invalid currency data provided.");
                return BadRequest();
            }

            try
            {
                var currencyRepository = _unitOfWork.GetRepository<Currency>();
                var existingCurrency = await currencyRepository.GetById(id);
                if (existingCurrency == null)
                {
                    _logger.LogWarning($"Currency with ID {id} not found.");
                    return NotFound();
                }

                _mapper.Map(currencyDto, existingCurrency);
                _unitOfWork.CreateTransaction();
                await currencyRepository.Update(existingCurrency);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                _logger.LogInformation($"Currency with ID {id} updated successfully.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                _unitOfWork.Rollback();
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            //try
            //{
            //    _logger.LogInformation($"Deleting currency with ID {id}...");

            //    var currencyRepository = _unitOfWork.GetRepository<Currency>();
            //    var deleted = await currencyRepository.Delete(id);
            //    if (!deleted)
            //    {
            //        _logger.LogWarning($"Currency with ID {id} not found.");
            //        return NotFound();
            //    }

            //    await _unitOfWork.SaveChangesAsync();

            //    _logger.LogInformation($"Currency with ID {id} deleted successfully.");

            //    return NoContent();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning(ex.Message);
            //    _unitOfWork.Rollback();
            //    return NotFound(ex.Message);
            //}
            try
            {
                var command = new DeleteCurrencyCommand() { Id = id };
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // Non Generic Repository method

        [HttpGet("currencies/{code}")]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrenciesByCode([FromServices] ICurrencyRepository currencyRepository, string code)
        {
            _logger.LogInformation($"Retrieving currencies by code: {code}...");

            if (string.IsNullOrEmpty(code))
            {
                _logger.LogWarning("Invalid currency code provided.");
                return NotFound();
            }

            var currencies = await currencyRepository.GetCurrenciesByCodeFrom(code);

            if (currencies == null)
            {
                _logger.LogInformation($"No currencies found with code {code}.");
                return NotFound($"No currencies found with code {code}");
            }

            var currencyDtos = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);

            _logger.LogInformation($"Retrieved {currencyDtos.Count()} currencies with code {code}.");

            return Ok(currencyDtos);
        }
        
        // Create Method to get data from openApi
        [HttpGet("openapi")]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOpenAPiData([FromServices] ICurrencyRepository icurrencyRepository, [FromQuery] string to, [FromQuery] string from, [FromQuery] decimal amount)
        {
            //try
            //{
            //    //Check the internet connectivity 
            //    string host = "https://www.google.com";
            //    Ping p = new Ping();
            //    PingReply reply = p.Send(host, 3000);
            //}
            //catch (Exception ex)
            //{
            //    return NotFound(new { internet = "No Internet Connectivity. Please connect to internet and retry" });
            //}
            

            _logger.LogInformation($"Retrieving currencies by codeFrom: {from}, codeTo: {to}...");

            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || amount<=0)
            {
                _logger.LogWarning("Invalid currency code provided.");
                return NotFound();
            }

            var currencies = await icurrencyRepository.GetCurrencyConverter(to, from);
            
            if (currencies == null || currencies.Count()==0)
            {
                currencies = await icurrencyRepository.GetCurrencyConverter(from, to);
                if (currencies == null || currencies.Count() == 0)
                {
                    if (!OpenAPiService.CheckForInternetConnection())
                    {
                        return NotFound(new { internet = "No Internet Connectivity. Please connect to internet and retry" });
                    }
                    var openApiService = new OpenAPiService();
                    // Response format
                    /*
                        "new_amount": 1141.78,
                        "new_currency": "PKR",
                        "old_currency": "USD",
                        "old_amount": 4.0
                     */
                    var response = await openApiService.GetOpenCurrency(to, from, amount.ToString());
                    var jsonObject = JObject.Parse(response);
                    decimal conversionRate = (decimal)jsonObject["new_amount"] / (decimal)jsonObject["old_amount"];
                    
                    Currency currency = new Currency();
                    currency.CurrencyCodeFrom = from;
                    currency.CurrencyCodeTo = to;
                    currency.ConversionRate = conversionRate;

                    var currencyRepository = _unitOfWork.GetRepository<Currency>();
                    _unitOfWork.CreateTransaction();
                    await currencyRepository.Create(currency);
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();
                    return Ok(response);
                }
                else
                {
                    var currency = currencies.First();
                    decimal newAmount = amount / (decimal)currency.ConversionRate;
                    return Ok(new { new_amount = newAmount, new_currency =to, old_currency = from, old_amount = amount });

                }
            }
            else
            {
                var currency = currencies.First();
                decimal newAmount = amount * (decimal)currency.ConversionRate;
                return Ok(new { new_amount = newAmount, new_currency = to, old_currency = from, old_amount = amount });
            }
            
        }

    }
}




//Without Using UnitOfWork
/*
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using trackingApi.Data;
using trackingApi.Dtos;
using trackingApi.GenericRepository;
using trackingApi.Models;
using trackingApi.NonGenericRepository;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace trackingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IssueDbContext _context;
        private readonly IMapper _mapper;
        public CurrencyController(IssueDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<CurrencyDto>> Get([FromServices] IGenericRepository<Currency> genericRepository)
        {
            var currencies = await genericRepository.GetAll();
            return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromServices] IGenericRepository<Currency> genericRepository, int id)
        {
            var currency = await genericRepository.GetById(id);
            if (currency == null)
            {
                return NotFound();
            }

            var currencyDto = _mapper.Map<CurrencyDto>(currency);
            return Ok(currencyDto);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromServices] IGenericRepository<Currency> genericRepository, CurrencyDto currencyDto)
        {
            if (currencyDto == null)
            {
                return BadRequest();
            }
            var currency = _mapper.Map<Currency>(currencyDto);
            await genericRepository.Create(currency);
            var createdCurrencyDto = _mapper.Map<CurrencyDto>(currency);
            return CreatedAtAction(nameof(GetById), new { id = currency.Id }, createdCurrencyDto);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromServices] IGenericRepository<Currency> genericRepository, CurrencyDto currencyDto, int id)
        {
            if (currencyDto == null)
            {
                return BadRequest();
            }

            var existingCurrency = await genericRepository.GetById(id);
            if (existingCurrency == null)
            {
                return NotFound();
            }

            // Update the properties of the existing currency entity with the values from the DTO
            existingCurrency.CurrencyCodeTo = currencyDto.CurrencyCodeTo;
            existingCurrency.CurrencyCodeFrom = currencyDto.CurrencyCodeFrom;
            existingCurrency.ConvertedAmount = currencyDto.ConvertedAmount;
            existingCurrency.ConvertFromAmount = currencyDto.ConvertFromAmount;
            // Update other properties as needed

            var updatedCurrency = await genericRepository.Update(existingCurrency);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromServices] IGenericRepository<Currency> genericRepository, int id)
        {
            var deleted = await genericRepository.Delete(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        // Non Generic Repository
        [HttpGet("currencies/{code}")]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrenciesByCode([FromServices] ICurrencyRepository currencyRepository, string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return NotFound();
            }

            var currencies = await currencyRepository.GetCurrenciesByCode(code);

            if (currencies == null)
            {
                return NotFound($"No currencies found with code {code}");
            }

            var currencyDtos = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
            return Ok(currencyDtos);
        }

        //Without Using DTOs
        //[HttpGet]
        //public async Task<IEnumerable<Currency>> Get([FromServices] IGenericRepository<Currency> genericRepository)
        //{
        //    return await genericRepository.GetAll();
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(Currency), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Currency), StatusCodes.Status404NotFound)]

        //public async Task<IActionResult> GetById([FromServices] IGenericRepository<Currency> genericRepository, int id)
        //{
        //    var issue = await genericRepository.GetById(id);
        //    if (issue == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(issue);
        //}

        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //public async Task<IActionResult> Create([FromServices] IGenericRepository<Currency> genericRepository, Currency currency)
        //{
        //    var created = await genericRepository.Create(currency, currency.Id);
        //    if (!created) { return BadRequest(); }
        //    return CreatedAtAction(nameof(GetById), new { id = currency.Id }, currency);
        //}


        //[HttpPut("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Update([FromServices] IGenericRepository<Currency> genericRepository, Currency currency, int id)
        //{
        //    if (currency == null || id != currency.Id)
        //    {
        //        return BadRequest();
        //    }
        //    var entity = await genericRepository.Update(currency);

        //    return NoContent();
        //}


        //[HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> Delete([FromServices] IGenericRepository<Currency> genericRepository, int id)
        //{
        //    var created = await genericRepository.Delete(id);
        //    if (!created)
        //    {
        //        return NotFound();
        //    }
        //    return NoContent();
        //}

        ////Non Generic Repository
        //[HttpGet("currencies/{code}")]
        //[ProducesResponseType(typeof(Currency), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Currency), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetCurrenciesByCode([FromServices] ICurrencyRepository currencyRepository, string code)
        //{
        //    if(string.IsNullOrEmpty(code)) return NotFound();
        //    var currencies = await currencyRepository.GetCurrenciesByCode(code);
        //    if (currencies == null)
        //    {
        //        return NotFound($"No currencies found with code {code}");
        //    }

        //    return Ok(currencies);
        //}

    }
}
*/