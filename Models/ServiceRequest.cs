using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Prog7312PoePart1.Models
{
    public enum RequestStatus
    {
        Waiting,
        Pending, 
        Resolved
    }

    public class ServiceRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); 

        [Required, StringLength(150)]
        public string Title { get; set; }

        [Required, StringLength(1000)]
        public string Description { get; set; }

        [Required, StringLength(150)]
        public string Location { get; set; }

        [Required, StringLength(100)]
        public string Category { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Waiting;

        public int Priority { get; set; } = 3;
        public string PriorityText => Priority switch
        {
            1 => "High",
            2 => "Medium",
            3 => "Low"
        };

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<string> History { get; set; } = new();

        public string? MediaPath { get; set; } 
    }
}
