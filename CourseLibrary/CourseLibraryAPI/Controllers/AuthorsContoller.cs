using CourseLibraryAPI.Services;
using CourseLibraryAPI.Helpers;
using CourseLibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;

namespace CourseLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsContoller : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        public AuthorsContoller(ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper )
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(CourseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(Mapper));    
        }

        [HttpGet()]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors()
        {
            var authorsFromRepo = _courseLibraryRepository.GetAuthors();
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));  
        }

        [HttpGet("{authorId}")] 
        public IActionResult GetAutor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();  
            }
           
            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
        }
    }
}
