using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using trackingApi.Data;
using trackingApi.Dtos;
using trackingApi.GenericRepository;
using trackingApi.Models;
using trackingApi.NonGenericRepository;
using trackingApi.OpenAPiService1;
using trackingApi.UnitOfWork;

namespace trackingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WeatherController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        /*
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Getting all weather...");

            var weatherRepository = _unitOfWork.GetRepository<Weather>();
            var weatherEntities = await weatherRepository.GetAll();
            var weatherDtos = _mapper.Map<IEnumerable<WeatherDto>>(weatherEntities);

            return Ok(weatherDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Getting weather by ID: {id}...");

            var weatherRepository = _unitOfWork.GetRepository<Weather>();
            var weatherEntity = await weatherRepository.GetById(id);

            if (weatherEntity == null)
            {
                _logger.LogWarning($"Weather with ID {id} not found.");
                return NotFound();
            }

            var weatherDto = _mapper.Map<WeatherDto>(weatherEntity);
            return Ok(weatherDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(WeatherDto weatherDto)
        {
            _logger.LogInformation("Creating a new weather...");

            if (weatherDto == null)
            {
                _logger.LogWarning("Invalid weather data provided.");
                return BadRequest();
            }

            var weatherEntity = _mapper.Map<Weather>(weatherDto);
            var weatherRepository = _unitOfWork.GetRepository<Weather>();
            await weatherRepository.Create(weatherEntity);
            await _unitOfWork.SaveChangesAsync();

            weatherDto = _mapper.Map<WeatherDto>(weatherEntity);
            return CreatedAtAction(nameof(GetById), new { id = weatherEntity.Id }, weatherDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(WeatherDto weatherDto, int id)
        {
            _logger.LogInformation($"Updating weather with ID {id}...");

            if (weatherDto == null)
            {
                _logger.LogWarning("Invalid weather data provided.");
                return BadRequest();
            }

            var weatherRepository = _unitOfWork.GetRepository<Weather>();
            var existingWeather = await weatherRepository.GetById(id);

            if (existingWeather == null)
            {
                _logger.LogWarning($"Weather with ID {id} not found.");
                return NotFound();
            }

            existingWeather.CurrentWeather = weatherDto.CurrentWeather;
            existingWeather.City = weatherDto.City;
            //existingWeather.City = weatherDto.Country;

            await weatherRepository.Update(existingWeather);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Deleting weather with ID {id}...");

            var weatherRepository = _unitOfWork.GetRepository<Weather>();
            var deleted = await weatherRepository.Delete(id);

            if (!deleted)
            {
                _logger.LogWarning($"Weather with ID {id} not found.");
                return NotFound();
            }

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        */
        // Create Method to get data from openApi
        [HttpGet("openapi")]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOpenAPiData([FromServices] IWeatherRepository iweatherRepository, [FromQuery] string city)
        {
            _logger.LogInformation($"Retrieving news by city: {city} ...");

            if (string.IsNullOrEmpty(city))
            {
                _logger.LogWarning("Invalid currency code provided.");
                return NotFound();
            }

            var weather = await iweatherRepository.GetWeatherByCity(city);
            if(weather == null || weather.Count()==0)
            {
                if (!OpenAPiService.CheckForInternetConnection())
                {
                    return NotFound(new { internet = "No Internet Connectivity. Please connect to internet and retry" });
                }
                var openApiService = new OpenAPiService();
                var response = await openApiService.GetOpenWeather(city);
                if (response == null)
                {
                    return NotFound();
                }
                var jsonObject = JObject.Parse(response);
                Weather weather1 = new()
                {
                    City = city,
                    CurrentWeather = JsonConvert.SerializeObject(jsonObject["weather"]),
                    Latitude = JsonConvert.SerializeObject(jsonObject["coord"]["lat"]),
                    Longitude = JsonConvert.SerializeObject(jsonObject["coord"]["lon"])
                };

                var genericRepository = _unitOfWork.GetRepository<Weather>();
                _unitOfWork.CreateTransaction();
                await genericRepository.Create(weather1);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                var weather1Dto = _mapper.Map<WeatherDto>(weather1);
                return Ok(JsonConvert.SerializeObject(weather1Dto));
            }
            var weatherDto = _mapper.Map<IEnumerable<WeatherDto>>(weather);
            return Ok(JsonConvert.SerializeObject(weatherDto));
        }
    }
}



/*
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using trackingApi.Data;
using trackingApi.Dtos;
using trackingApi.GenericRepository;
using trackingApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace trackingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IssueDbContext _context;
        private readonly IMapper _mapper;

        public WeatherController(IssueDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherDto>> Get([FromServices] IGenericRepository<Weather> genericRepository)
        {
            var weatherEntities = await genericRepository.GetAll();
            return _mapper.Map<IEnumerable<WeatherDto>>(weatherEntities);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromServices] IGenericRepository<Weather> genericRepository, int id)
        {
            var weatherEntity = await genericRepository.GetById(id);
            if (weatherEntity == null)
            {
                return NotFound();
            }
            var weatherDto = _mapper.Map<WeatherDto>(weatherEntity);
            return Ok(weatherDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromServices] IGenericRepository<Weather> genericRepository, WeatherDto weatherDto)
        {
            if (weatherDto == null)
            {
                return BadRequest();
            }
            var weatherEntity = _mapper.Map<Weather>(weatherDto);
            await genericRepository.Create(weatherEntity);
            
            weatherDto = _mapper.Map<WeatherDto>(weatherEntity);
            return CreatedAtAction(nameof(GetById), new { id = weatherEntity.Id }, weatherDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromServices] IGenericRepository<Weather> genericRepository, WeatherDto weatherDto, int id)
        {
            if (weatherDto == null)
            {
                return BadRequest();
            }
            var existingWeather = await genericRepository.GetById(id);
            if (existingWeather == null)
            {
                return NotFound();
            }
            existingWeather.CurrentWeather = weatherDto.CurrentWeather;
            existingWeather.Location = weatherDto.Location;
            await genericRepository.Update(existingWeather);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromServices] IGenericRepository<Weather> genericRepository, int id)
        {
            var deleted = await genericRepository.Delete(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        //Without Using DTOs
        //[HttpGet]
        //public async Task<IEnumerable<Weather>> Get([FromServices] IGenericRepository<Weather> genericRepository)
        //{
        //    return await genericRepository.GetAll();
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(Weather), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Weather), StatusCodes.Status404NotFound)]

        //public async Task<IActionResult> GetById([FromServices] IGenericRepository<Weather> genericRepository, int id)
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
        //public async Task<IActionResult> Create([FromServices] IGenericRepository<Weather> genericRepository, Weather weather)
        //{
        //    var created = await genericRepository.Create(weather, weather.Id);
        //    if (!created) { return BadRequest(); }
        //    return CreatedAtAction(nameof(GetById), new { id = weather.Id }, weather);
        //}


        //[HttpPut("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Update([FromServices] IGenericRepository<Weather> genericRepository, Weather weather, int id)
        //{
        //    if (weather == null || id != weather.Id)
        //    {
        //        return BadRequest();
        //    }
        //    var entity = await genericRepository.Update(weather);

        //    return NoContent();
        //}


        //[HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> Delete([FromServices] IGenericRepository<Weather> genericRepository, int id)
        //{
        //    var created = await genericRepository.Delete(id);
        //    if (!created)
        //    {
        //        return NotFound();
        //    }
        //    return NoContent();
        //}
    }
}
*/