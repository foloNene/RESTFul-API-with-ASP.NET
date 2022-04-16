using CourseLibraryAPI.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CourseLibraryAPI.Models
{
    [CourseTitleMustBeDifferentFromDescriptionAttribute(
        ErrorMessage = "Title must be differnt from description.")]
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a title")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 1500 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "You should fill out a description")]
        [MaxLength(1500, ErrorMessage = "The desription shouldn't have more than 1500 characters.")]
        public virtual string Description { get; set; }
    }
}
