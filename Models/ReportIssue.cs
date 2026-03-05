using System;
using System.ComponentModel.DataAnnotations;

namespace Prog7312PoePart1.Models
{

    public class ReportIssue
    {
        [Key] // Primary key
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateReported { get; set; } = DateTime.Now;

        public string? MediaPath { get; set; }
    }
}
