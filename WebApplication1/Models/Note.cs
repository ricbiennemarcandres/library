using System.ComponentModel.DataAnnotations;

namespace library.Models
{
    public class Note
    {
        public long NoteId { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Note Body")]
        public string? Body { get; set; }

        [Display(Name = "Date Created")]
        public DateOnly CreatedOn { get; set; }
    }
}