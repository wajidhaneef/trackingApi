using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackingApi.Data;
using trackingApi.Models;
using trackingApi.GenericRepository;
namespace trackingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IssueDbContext _context;

        public IssueController(IssueDbContext context) => _context = context;



        //[HttpGet]
        //public async Task<IEnumerable<Issue>> Get([FromServices] IGenericRepository<Issue> genericRepository)
        //{
        //    return await genericRepository.GetAll();
        //}
        ////[HttpGet]
        ////public async Task<IEnumerable<Issue>> Get()
        ////{
        ////    //HttpClient client = new HttpClient();
        ////    //client.BaseAddress = new Uri("https://weatherapi-com.p.rapidapi.com");
        ////    //client.DefaultRequestHeaders.Accept.Clear();
        ////    //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        ////    return await _context.Issues.ToListAsync();
        ////}
        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(Issue), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Issue), StatusCodes.Status404NotFound)]

        //public async Task<IActionResult> GetById([FromServices] IGenericRepository<Issue> genericRepository, int id)
        //{
        //    var issue = await genericRepository.GetById(id);
        //    if (issue == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(issue);
        //}
        ////public async Task<IActionResult> GetById(int id) 
        ////{
        ////    var issue = await _context.Issues.FindAsync(id);
        ////    return issue==null? NotFound(): Ok(issue);
        ////}
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //public async Task<IActionResult> Create([FromServices] IGenericRepository<Issue> genericRepository, Issue issue)
        //{
        //    if (issue==null) { 
        //        return BadRequest();
        //    }
        //    await genericRepository.Create(issue);
            
        //    return CreatedAtAction(nameof(GetById), new { id = issue.Id }, issue);
        //}
        ////public async Task<IActionResult> Create(Issue issue)
        ////{
        ////    await _context.Issues.AddAsync(issue);
        ////    await _context.SaveChangesAsync();

        ////    return CreatedAtAction(nameof(GetById), new {id = issue.Id}, issue);
        ////}

        //[HttpPut("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Update([FromServices]IGenericRepository<Issue> genericRepository, Issue issue, int id)
        //{
        //    if(issue == null || id != issue.Id)
        //    {
        //        return BadRequest();
        //    }
        //    var entity = await genericRepository.Update(issue);

        //    return NoContent();
        //}
        ////public async Task<IActionResult> Update(int id, Issue issue)
        ////{
        ////    if (id != issue.Id) return BadRequest();
        ////    _context.Entry(issue).State = EntityState.Modified;
        ////    await _context.SaveChangesAsync();

        ////    return NoContent();
        ////}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> Delete([FromServices] IGenericRepository<Issue> genericRepository, int id)
        //{
        //    var created = await genericRepository.Delete(id);
        //    if (!created)
        //    {
        //        return NotFound();
        //    }
        //    return NoContent();
        //}
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var issueToDelete = await _context.Issues.FindAsync(id);
        //    if (issueToDelete==null) return NotFound();
        //    _context.Entry(issueToDelete).State = EntityState.Deleted;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
    }
}
