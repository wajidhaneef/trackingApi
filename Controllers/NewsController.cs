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
    public class NewsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsController> _logger;

        public NewsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<NewsController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Getting all news...");

            var newsRepository = _unitOfWork.GetRepository<News>();
            var newsList = await newsRepository.GetAll();
            var newsDtoList = _mapper.Map<IEnumerable<NewsDto>>(newsList);

            return Ok(newsDtoList);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(News), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Getting news by ID: {id}...");

            var newsRepository = _unitOfWork.GetRepository<News>();
            var news = await newsRepository.GetById(id);

            if (news == null)
            {
                _logger.LogWarning($"News with ID {id} not found.");
                return NotFound();
            }

            var newsDto = _mapper.Map<NewsDto>(news);
            return Ok(newsDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(NewsDto newsDto)
        {
            _logger.LogInformation("Creating a new news...");

            if (newsDto == null)
            {
                _logger.LogWarning("Invalid news data provided.");
                return BadRequest();
            }

            try
            {
                _unitOfWork.CreateTransaction();
                var news = _mapper.Map<News>(newsDto);
                var newsRepository = _unitOfWork.GetRepository<News>();
                await newsRepository.Create(news);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                var createdDto = _mapper.Map<NewsDto>(news);
                return CreatedAtAction(nameof(GetById), new { id = news.Id }, createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                _unitOfWork.Rollback();
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(NewsDto newsDto, int id)
        {
            _logger.LogInformation($"Updating news with ID {id}...");

            if (newsDto == null)
            {
                _logger.LogWarning("Invalid news data provided.");
                return BadRequest();
            }

            try
            {
                _unitOfWork.CreateTransaction();
                var newsRepository = _unitOfWork.GetRepository<News>();
                var existingNews = await newsRepository.GetById(id);

                if (existingNews == null)
                {
                    _logger.LogWarning($"News with ID {id} not found.");
                    return NotFound();
                }

                existingNews.Title = newsDto.Title;
                existingNews.Articles = newsDto.Articles;

                await newsRepository.Update(existingNews);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogWarning(ex.Message); 
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting news with ID {id}...");

                _unitOfWork.CreateTransaction();
                var newsRepository = _unitOfWork.GetRepository<News>();
                var deleted = await newsRepository.Delete(id);

                if (!deleted)
                {
                    _logger.LogWarning($"News with ID {id} not found.");
                    return NotFound();
                }

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                _unitOfWork.Rollback();
                return BadRequest(ex.Message);
            }
        }
        
        // Create Method to get data from openApi
        [HttpGet("openapi")]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOpenAPiData([FromServices] INewsRepository inewsRepository, [FromQuery] string title)
        {
            _logger.LogInformation($"Retrieving news by title: {title} ...");

            if (string.IsNullOrEmpty(title))
            {
                _logger.LogWarning("Invalid currency code provided.");
                return NotFound();
            }

            var newss = await inewsRepository.GetNewsByTitle(title);

            if (newss == null || newss.Count() == 0)
            {
                if (!OpenAPiService.CheckForInternetConnection())
                {
                    return NotFound(new { internet = "No Internet Connectivity. Please connect to internet and retry" });
                }
                var openAPiService = new OpenAPiService();
                var response = await openAPiService.GetOpenNews(title);
                var jsonObject = JObject.Parse(response);
                if (response == null)
                {
                    return NotFound();
                }
                News newsTemp = new()
                {
                    Title = title,
                    //Use "results" for https://newsdata.io/api/1/news?apikey=pub_2241410d88c2afec96b134fececf4df9a330f&q=
                    // Use "articles" for https://newsapi.org/v2/everything?q=
                    Articles = JsonConvert.SerializeObject(jsonObject["results"]),
                    NumOfArticles = (int)jsonObject["totalResults"]
                };

                var newsRepository = _unitOfWork.GetRepository<News>();
                _unitOfWork.CreateTransaction();
                await newsRepository.Create(newsTemp);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                var newsTempDto = _mapper.Map<NewsDto>(newsTemp);
                return Ok(JsonConvert.SerializeObject(newsTempDto));
            }
            //var newsDtoList = _mapper.Map<IEnumerable<NewsDto>>(newsList);
            var newsDto = _mapper.Map<IEnumerable<NewsDto>>(newss);
            return Ok(JsonConvert.SerializeObject(newsDto));
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
    public class NewsController : ControllerBase
    {
        private readonly IssueDbContext _context;
        private readonly IMapper _mapper;

        public NewsController(IssueDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IEnumerable<NewsDto>> Get([FromServices] IGenericRepository<News> genericRepository)
        {
            var newsList = await genericRepository.GetAll();
            return _mapper.Map<IEnumerable<NewsDto>>(newsList);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(News), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromServices] IGenericRepository<News> genericRepository, int id)
        {
            var news = await genericRepository.GetById(id);
            if (news == null)
            {
                return NotFound();
            }
            var newsDto = _mapper.Map<NewsDto>(news);
            return Ok(newsDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromServices] IGenericRepository<News> genericRepository, NewsDto newsDto)
        {
            if (newsDto == null)
            {
                return BadRequest();
            }
            var news = _mapper.Map<News>(newsDto);
            await genericRepository.Create(news);
            
            var createdDto = _mapper.Map<NewsDto>(news);
            return CreatedAtAction(nameof(GetById), new { id = news.Id }, createdDto);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromServices] IGenericRepository<News> genericRepository, NewsDto newsDto, int id)
        {
            if (newsDto == null)
            {
                return BadRequest();
            }

            var existingNews = await genericRepository.GetById(id);
            if (existingNews == null)
            {
                return NotFound();
            }
            existingNews.Title = newsDto.Title;
            existingNews.Articles = newsDto.Articles;
            await genericRepository.Update(existingNews);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromServices] IGenericRepository<News> genericRepository, int id)
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
        //public async Task<IEnumerable<News>> Get([FromServices] IGenericRepository<News> genericRepository)
        //{
        //    return await genericRepository.GetAll();
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(News), StatusCodes.Status404NotFound)]

        //public async Task<IActionResult> GetById([FromServices] IGenericRepository<News> genericRepository, int id)
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
        //public async Task<IActionResult> Create([FromServices] IGenericRepository<News> genericRepository, News news)
        //{
        //    var created = await genericRepository.Create(news, news.Id);
        //    if (!created) { return BadRequest(); }
        //    return CreatedAtAction(nameof(GetById), new { id = news.Id }, news);
        //}


        //[HttpPut("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Update([FromServices] IGenericRepository<News> genericRepository, News news, int id)
        //{
        //    if (news == null || id != news.Id)
        //    {
        //        return BadRequest();
        //    }
        //    var entity = await genericRepository.Update(news);

        //    return NoContent();
        //}


        //[HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> Delete([FromServices] IGenericRepository<News> genericRepository, int id)
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