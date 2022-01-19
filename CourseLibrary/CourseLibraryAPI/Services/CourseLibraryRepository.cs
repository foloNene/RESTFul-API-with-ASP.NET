using CourseLibraryAPI.DbContexts;
using CourseLibraryAPI.Entities;
using CourseLibraryAPI.ResourceParameters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibraryAPI.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        private readonly CourseLibraryContext _context;

        public CourseLibraryRepository(CourseLibraryContext context )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course); 
        }         

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }
  
        public async Task<Course> GetCourseAsync(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return await _context.Courses
              .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefaultAsync();
        }

        public async Task <IEnumerable<Course>> GetCoursesAsync(Guid authorId)
        {  
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _context.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToListAsync();
        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
        }


        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        public async Task <Author> GetAuthorAsync(Guid authorId)

        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task <IEnumerable<Author>> GetAuthorsAsync(AuthorsResourceParameters authorsResourceParameters)
        {
            if (authorsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(authorsResourceParameters));
            }

            if (string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory)
                && string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {

                return await _context.Authors.ToListAsync();
            }

            var collection = _context.Authors as IQueryable<Author>;
             
            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory))
            {
                var mainCategory = authorsResourceParameters.MainCategory.Trim();
                collection = collection.Where(a => a.MainCategory == mainCategory);
            }
            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                var searchQuery = authorsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(searchQuery)
                || a.FirstName.Contains(searchQuery)
                || a.LastName.Contains(searchQuery));
            }
            return await collection.ToListAsync();
        }
        public async Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return await _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToListAsync();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        //public bool Save()
        
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >=0);  
            //return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
               // dispose resources when needed
            }
        }

        
    }
}
