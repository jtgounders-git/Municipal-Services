using System;
using System.ComponentModel.DataAnnotations;

namespace Prog7312PoePart1.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; }

        [Required, StringLength(100)]
        public string Category { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public bool IsAnnouncement { get; set; } = false;

        public int Priority { get; set; }  // for priority queue usage
    }
}
