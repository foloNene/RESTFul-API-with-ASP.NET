﻿using AutoMapper;
using CourseLibraryAPI.Models;
using CourseLibraryAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
              
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(CourseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(Mapper));
        }

        [HttpGet(Name ="GetCoursesForAuthor")]
        [HttpHead]
        [ResponseCache(Duration = 120)]
        public async Task <ActionResult<IEnumerable<CourseDto>>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var coursesForAuthorFromRepo = await _courseLibraryRepository.GetCoursesAsync(authorId);
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(coursesForAuthorFromRepo));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        public async Task<ActionResult<CourseDto>> GetCourseAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

            if(courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CourseDto>(courseForAuthorFromRepo));
        }

        [HttpPost(Name = "CreateCourseForAuthor")]
        public async Task <ActionResult<CourseDto>> CreateCourseForAuthor(
            Guid authorId, CourseForCreationDto course) 
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = _mapper.Map<Entities.Course>(course);
            _courseLibraryRepository.AddCourse(authorId, courseEntity);
             await _courseLibraryRepository.SaveChangesAsync();

            var courseToReturn = _mapper.Map<CourseDto>(courseEntity);
            return CreatedAtRoute("GetCourseForAuthor",
               new{ authorId = authorId, courseId = courseToReturn.Id},
               courseToReturn);
        }

        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateCourseForAuthor(Guid authorId, 
            Guid courseId,
            CourseForUpdateDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseForAuthorFromRepo = await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

            if(courseForAuthorFromRepo == null)
            {
                var courseToAdd = _mapper.Map<Entities.Course>(course);
                courseToAdd.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToAdd);

               await _courseLibraryRepository.SaveChangesAsync();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId, courseId = courseToReturn.Id},
                    courseToReturn);
            }

            //map the entity to a CourseForUpdateDto
            //apply the updated field value to the sto
            //map the courseForUpdateDto back to an entity
            _mapper.Map(course, courseForAuthorFromRepo);

            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

             await _courseLibraryRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public async Task<ActionResult> PartiallyUpdateCourseForAuthor(Guid authorId,
            Guid courseId,
            JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo =await _courseLibraryRepository.GetCourseAsync(authorId, courseId);

            if(courseForAuthorFromRepo == null)
            {
                var courseDto = new CourseForUpdateDto();
                patchDocument.ApplyTo(courseDto, ModelState);

                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }
                var courseToAdd = _mapper.Map<Entities.Course>(courseDto);
                courseToAdd.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToAdd);
               await  _courseLibraryRepository.SaveChangesAsync();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor",
                new {authorId, courseId = courseToReturn.Id},
                courseToReturn);
            }

            var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseForAuthorFromRepo);
            //add validation
            patchDocument.ApplyTo(courseToPatch, ModelState);

            if (!TryValidateModel(courseToPatch))
            {
                 return ValidationProblem(ModelState);
            }

            _mapper.Map(courseToPatch, courseForAuthorFromRepo);

              _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

             await _courseLibraryRepository.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{courseId}")]
        public async Task<ActionResult> DeleteCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo =await  _courseLibraryRepository.GetCourseAsync(authorId, courseId);

            if (courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            _courseLibraryRepository.DeleteCourse(courseForAuthorFromRepo);
            await _courseLibraryRepository.SaveChangesAsync();

            return NoContent();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

    }
}
  