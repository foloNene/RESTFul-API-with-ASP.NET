using CourseLibraryAPI.Services;
using CourseLibraryAPI.Helpers;
using CourseLibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibraryAPI.ResourceParameters;
using System.Threading.Tasks;

namespace CourseLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsContoller : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        public AuthorsContoller(ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(CourseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(Mapper));
        }

        [HttpGet()]
        [HttpHead]

        public async Task <ActionResult<IEnumerable<AuthorDto>>> GetAuthors(
          [FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            var authorsFromRepo = await _courseLibraryRepository.GetAuthorsAsync(authorsResourceParameters);
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));  
        }

        [HttpGet("{authorId}", Name ="GetAuthor")]
        public async Task<IActionResult> GetAutor(Guid authorId)
        {
            var authorFromRepo = await _courseLibraryRepository.GetAuthorAsync(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();  
            }
           
            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
        }

        [HttpPost]

        //public async Task<IActionResult<AuthorDTO>> CreateAuthor(AuthorForCreationDto author)
        public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorForCreationDto author)
        {
           
                var authorEntity = _mapper.Map<Entities.Author>(author);
                _courseLibraryRepository.AddAuthor(authorEntity);
                
                 await _courseLibraryRepository.SaveChangesAsync();

                var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
                return CreatedAtRoute("GetAuthor",
                    new { authorId = authorToReturn.Id },
                    authorToReturn);
           
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();    
        }

        [HttpDelete("{authorId}")]

        public async Task<ActionResult> DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = await _courseLibraryRepository.GetAuthorAsync(authorId);

            if(authorFromRepo == null)
            {
                return NotFound();
            }

            _courseLibraryRepository.DeleteAuthor(authorFromRepo);
            await _courseLibraryRepository.SaveChangesAsync();

            return NoContent();
        }

    }
}
