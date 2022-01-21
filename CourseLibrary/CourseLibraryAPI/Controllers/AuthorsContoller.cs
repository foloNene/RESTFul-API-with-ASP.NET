using CourseLibraryAPI.Services;
using CourseLibraryAPI.Helpers;
using CourseLibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibraryAPI.ResourceParameters;
using System.Threading.Tasks;
using System.Text.Json;

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

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        public async Task <ActionResult<IEnumerable<AuthorDto>>> GetAuthorsAsync(
          [FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            var authorsFromRepo = await _courseLibraryRepository.GetAuthorsAsync(authorsResourceParameters);

            var previousPageLink = authorsFromRepo.HasPrevious ?
                CreateAuthorsResourceUri(authorsResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = authorsFromRepo.HasNext ?
                CreateAuthorsResourceUri(authorsResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var paginationMetadata = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

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

        private string CreateAuthorsResourceUri(
            AuthorsResourceParameters authorsResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            PageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                       new
                       {
                           orderby = authorsResourceParameters.OrderBy,
                           pageNumber = authorsResourceParameters.PageNumber + 1,
                           PageSize = authorsResourceParameters.PageSize,
                           mainCategory = authorsResourceParameters.MainCategory,
                           searchQuery = authorsResourceParameters.SearchQuery
                       });
                default: 
                    return Url.Link("GetAuthors",
                        new
                        {
                            orderby = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber,
                            PageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        }); 
            }
             
        }

    }
}
