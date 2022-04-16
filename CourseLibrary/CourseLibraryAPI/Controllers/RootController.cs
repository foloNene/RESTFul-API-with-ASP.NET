using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CourseLibraryAPI.Models;

namespace CourseLibraryAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase 
    {
        [HttpGet(Name = "GetRook")]
        public IActionResult GetRoot()
        {
            // create link for root
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetRoot", new { }),
                "self",
                "GET"));

            links.Add(
                new LinkDto(Url.Link("GetAuthors", new { }),
                "authors",
                "GET"));

            links.Add(
                new LinkDto(Url.Link("GetRoot", new { }),
                "create_author",
                "POST"));

            return Ok(links);
        }

    }
}
