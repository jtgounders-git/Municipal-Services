using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog7312PoePart1.Data;
using Prog7312PoePart1.Models;

namespace Prog7312PoePart1.Controllers
{
    public class EventsController : Controller
    {
        // The database context (Entity Framework Core, backed by SQLite in this project)
        private readonly ApplicationDbContext _context;

        // -------------------- DATA STRUCTURES USED --------------------

        // SortedDictionary<Date, List<Event>>
        private static SortedDictionary<DateTime, List<Event>> _eventsByDate = new();

        // HashSet<string>
        private static HashSet<string> _categories = new();

        // Queue<Event>
        private static Queue<Event> _recommendedEvents = new();

        // PriorityQueue<Event, int>
        private static PriorityQueue<Event, int> _priorityEvents = new();

        // Stack<string>
        private static Stack<string> _recentSearches = new();

        // --------------------------------------------------------------

        // Controller constructor
        public EventsController(ApplicationDbContext context)
        {
            _context = context;

            // If the database is empty, populate it with seed data
            if (!_context.Events.Any())
                SeedDummyData();

            // Load data structures from the database when the controller is created
            LoadDataStructures();
        }

        // -------------------- MAIN PAGE LOGIC --------------------
        // Displays all events with optional search, category, and date filtering
        public async Task<IActionResult> Index(string searchTerm, string categoryFilter, DateTime? startDate, DateTime? endDate)
        {
            // Start with all events from the database
            var query = _context.Events.AsQueryable();

            // Local flags to check what type of filter is being applied
            bool hasSearch = !string.IsNullOrWhiteSpace(searchTerm);
            bool hasCategory = !string.IsNullOrWhiteSpace(categoryFilter);
            bool hasDateRange = startDate.HasValue && endDate.HasValue;

            // -------- SEARCH FILTER --------
            if (hasSearch)
            {
                string lowerSearch = searchTerm.ToLower();

                // Filter by event title (case-insensitive)
                query = query.Where(e => e.Title.ToLower().Contains(lowerSearch));

                // Record this search term for recommendations
                RecordSearch(searchTerm);
            }

            // -------- CATEGORY FILTER --------
            if (hasCategory)
            {
                string lowerCategory = categoryFilter.ToLower();

                // Filter by category (case-insensitive)
                query = query.Where(e => e.Category.ToLower() == lowerCategory);

                RecordSearch(categoryFilter);
            }

            // -------- DATE RANGE FILTER --------
            if (hasDateRange)
            {
                // Filters between two dates inclusively
                query = query.Where(e => e.Date.Date >= startDate.Value.Date && e.Date.Date <= endDate.Value.Date);
                RecordSearch($"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}");
            }
            else if (startDate.HasValue || endDate.HasValue)
            {
                // If user only entered one of the two dates, warn them
                TempData["DateWarning"] = "Please provide both a start and end date to filter by date range.";
            }

            // Execute the query and order the results by date
            var events = await query.OrderBy(e => e.Date).ToListAsync();

            // Update recommendations based on last user search
            UpdateRecommendations();

            // Top Priority now respects search filters
            ViewBag.TopPriority = GetTopPriorityEvents(5, events);
            ViewBag.Categories = _categories.OrderBy(c => c).ToList();
            ViewBag.Recommended = _recommendedEvents.Any() ? _recommendedEvents.ToList() : null;
            ViewBag.RecentSearches = _recentSearches.ToList();

            // Maintain search form values between reloads
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SelectedCategory"] = categoryFilter;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(events);
        }

        // -------------------- PRIORITY EVENTS --------------------
        // Retrieve top N priority events, optionally filtered by a given list
        private List<Event> GetTopPriorityEvents(int count, IEnumerable<Event>? filteredEvents = null)
        {
            var result = new List<Event>();
            var tempQueue = new PriorityQueue<Event, int>(_priorityEvents.UnorderedItems);

            // If a filtered list is provided, use it to limit results
            var filterSet = filteredEvents != null ? new HashSet<int>(filteredEvents.Select(e => e.Id)) : null;

            while (tempQueue.Count > 0 && result.Count < count)
            {
                if (tempQueue.TryDequeue(out var ev, out _))
                {
                    if (filterSet == null || filterSet.Contains(ev.Id))
                        result.Add(ev);
                }
            }

            return result;
        }

        // -------------------- RECOMMENDATION SYSTEM --------------------
        // Update recommended events based on the last 3 unique searches
        private void UpdateRecommendations()
        {
            _recommendedEvents.Clear();

            if (_recentSearches.Count == 0)
                return;

            // We'll collect recommendations from up to the last 3 unique search terms
            var searchTerms = _recentSearches.Distinct().Take(3).ToList();
            var recommendedList = new List<Event>();

            foreach (var term in searchTerms)
            {
                string lowerTerm = term.ToLower();
                var related = _context.Events
                    .AsEnumerable()
                    .Where(e =>
                        e.Title.ToLower().Contains(lowerTerm) ||
                        e.Category.ToLower().Contains(lowerTerm))
                    .Take(2) // take 2 from each search to keep results balanced
                    .ToList();

                recommendedList.AddRange(related);
            }

            // Avoid duplicates in the final recommendation queue
            foreach (var ev in recommendedList.DistinctBy(e => e.Id))
            {
                _recommendedEvents.Enqueue(ev);
            }
        }


        // -------------------- SEARCH HISTORY (STACK) --------------------
        // Records the last 3 search terms in a stack (LIFO)
        private void RecordSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return;

            // Maintain a maximum of 3 searches
            if (_recentSearches.Count >= 3)
                _recentSearches.Pop();

            _recentSearches.Push(term);
        }

        // -------------------- DATA INITIALIZATION --------------------
        // Loads data from the database into our data structures
        private void LoadDataStructures()
        {
            _eventsByDate.Clear();
            _categories.Clear();
            _priorityEvents.Clear();

            var events = _context.Events.ToList();

            // Populate each structure
            for (int i = 0; i < events.Count; i++)
            {
                var ev = events[i];

                // SortedDictionary → organize by event date
                if (!_eventsByDate.ContainsKey(ev.Date.Date))
                    _eventsByDate[ev.Date.Date] = new List<Event>();

                _eventsByDate[ev.Date.Date].Add(ev);

                // HashSet → store unique categories
                _categories.Add(ev.Category);

                // PriorityQueue → enqueue by descending priority
                _priorityEvents.Enqueue(ev, -ev.Priority);
            }
        }

        // -------------------- SEED DATA --------------------
        // Populates dummy events if the database is empty
        private void SeedDummyData()
        {
            var dummyEvents = new List<Event>
            {
                new Event { Title = "Community Clean-Up Day", Description = "Join us for a neighbourhood cleanup event.", Category = "Community", Date = DateTime.Now.AddDays(3), IsAnnouncement = false, Priority = 1 },
                new Event { Title = "Water Supply Maintenance", Description = "Scheduled water maintenance for the West region.", Category = "Utilities", Date = DateTime.Now.AddDays(1), IsAnnouncement = true, Priority = 4 },
                new Event { Title = "Youth Sports Tournament", Description = "Municipal sports day for local schools.", Category = "Sports", Date = DateTime.Now.AddDays(7), IsAnnouncement = false, Priority = 3 },
                new Event { Title = "Power Outage Update", Description = "Electricity outage expected in certain wards.", Category = "Utilities", Date = DateTime.Now.AddDays(2), IsAnnouncement = true, Priority = 5 },
                new Event { Title = "Heritage Day Celebration", Description = "Cultural celebration at the Town Hall.", Category = "Cultural", Date = DateTime.Now.AddDays(10), IsAnnouncement = false, Priority = 2 },
                new Event { Title = "Farmers Market", Description = "Weekly farmers market at the main square.", Category = "Community", Date = DateTime.Now.AddDays(5), IsAnnouncement = false, Priority = 1 },
                new Event { Title = "Public Library Renovation", Description = "Library closed for renovations until further notice.", Category = "Public Services", Date = DateTime.Now.AddDays(12), IsAnnouncement = true, Priority = 3 },
                new Event { Title = "Tech Career Expo", Description = "Networking event for IT and tech enthusiasts.", Category = "Education", Date = DateTime.Now.AddDays(8), IsAnnouncement = false, Priority = 2 },
                new Event { Title = "Music in the Park", Description = "Live concert series in Central Park.", Category = "Entertainment", Date = DateTime.Now.AddDays(4), IsAnnouncement = false, Priority = 2 },
                new Event { Title = "Recycling Awareness Campaign", Description = "Learn how to recycle effectively and protect the planet.", Category = "Environmental", Date = DateTime.Now.AddDays(6), IsAnnouncement = true, Priority = 3 }
            };

            _context.Events.AddRange(dummyEvents);
            _context.SaveChanges();
        }
    }
}
