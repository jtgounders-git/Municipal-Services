using Microsoft.EntityFrameworkCore;
using Prog7312PoePart1.Models;

namespace Prog7312PoePart1.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor takes options and passes them to the base DbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet represents the ReportIssue table in the database
        // Allows CRUD operations on ReportIssue entities
        public DbSet<ReportIssue> ReportIssues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
    }
}