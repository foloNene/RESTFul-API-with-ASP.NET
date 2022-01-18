using CourseLibraryAPI.Entities;
using CourseLibraryAPI.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseLibraryAPI.Services
{
    public interface ICourseLibraryRepository
    {    
        Task<IEnumerable<Course>> GetCoursesAsync(Guid authorId);
        Task<Course>GetCourseAsync(Guid authorId, Guid courseId);
        void AddCourse(Guid authorId, Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        //IEnumerable<Author> GetAuthors();
        Task<IEnumerable<Author>>GetAuthorsAsync();
        Task<IEnumerable<Author>> GetAuthorsAsync(AuthorsResourceParameters authorsResourceParameters);
        //IEnumerable<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters);
        Task<Author>GetAuthorAsync(Guid authorId);
        Task<IEnumerable<Author>>GetAuthorsAsync(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        bool AuthorExists(Guid authorId);

        Task<bool> SaveChangesAsync();
        //Task<bool> Save();
    }
}
