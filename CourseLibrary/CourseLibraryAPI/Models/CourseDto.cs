using System;

namespace CourseLibraryAPI.Models
{
    public class CourseDto
    {
        public Guid id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AuthorId  { get; set; }

    }
}
