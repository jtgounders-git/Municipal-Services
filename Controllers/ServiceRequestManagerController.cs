using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog7312PoePart1.Data;
using Prog7312PoePart1.Models;
using Prog7312PoePart1.Services;
using Prog7312PoePart1.DataStructures;

namespace Prog7312PoePart1.Controllers
{
    public class ServiceRequestManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ServiceRequestManager/GetFiltered
        [HttpGet]
        public async Task<IActionResult> GetFiltered(string search = "", string status = "", string category = "", string priority = "", string sort = "createdAsc")
        {
            var allRequests = await _context.ServiceRequests.AsNoTracking().ToListAsync();
            var manager = new ServiceRequestManager(allRequests);

            var filtered = manager.FilterRequests(search, status, category, priority);
            var finalList = manager.SortRequests(filtered, sort).ToList();

            var graph = manager.BuildRequestGraph(finalList);
            var mst = graph.PrimMST();

            var resultData = finalList.Select(r => new
            {
                id = r.Id,
                title = r.Title,
                description = r.Description,
                category = r.Category,
                location = r.Location,
                status = r.Status.ToString(),
                priority = r.Priority,
                createdAt = r.CreatedAt,
                mediaPath = r.MediaPath
            });

            var meta = new
            {
                total = finalList.Count,
                mstEdges = mst.Select(e => new { a = e.a, b = e.b, w = e.w }).ToList()
            };

            return Json(new { data = resultData, meta });
        }

        public IActionResult Visualise()
        {
            return View();
        }

        // GET: /ServiceRequestManager/VisualiseData
        [HttpGet]
        public async Task<IActionResult> VisualiseData()
        {
            var requests = await _context.ServiceRequests.ToListAsync();

            // Build MST edges
            var mstEdges = new List<object>();
            for (int i = 0; i < requests.Count; i++)
            {
                for (int j = i + 1; j < requests.Count; j++)
                {
                    int weight = 0;
                    if (requests[i].Category == requests[j].Category) weight += 1;
                    if (requests[i].Status == requests[j].Status) weight += 1;
                    weight += Math.Abs(requests[i].Priority - requests[j].Priority);

                    mstEdges.Add(new { a = requests[i].Title, b = requests[j].Title, w = weight });
                }
            }

            // Build stats
            var stats = new
            {
                totalRequests = requests.Count,
                byCategory = requests.GroupBy(r => r.Category).ToDictionary(g => g.Key, g => g.Count()),
                byStatus = requests.GroupBy(r => r.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()),
                byPriority = requests.GroupBy(r => r.Priority).ToDictionary(g => g.Key, g => g.Count())
            };

            return Json(new { meta = new { mstEdges, stats } });
        }
    }
}
