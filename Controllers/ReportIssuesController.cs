using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog7312PoePart1.Data;
using Prog7312PoePart1.Models;

namespace Prog7312PoePart1.Controllers
{
    public class ReportIssuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportIssuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReportIssues
        public async Task<IActionResult> Index()
        {
            // Fetch all reports from the database
            var reports = await _context.ReportIssues.ToListAsync();
            return View(reports);
        }

        // GET: ReportIssues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reportIssue = await _context.ReportIssues
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reportIssue == null) return NotFound();

            return View(reportIssue);
        }

        // GET: ReportIssues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReportIssues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportIssue reportIssue, IFormFile? mediaFile)
        {
            if (!ModelState.IsValid) return View(reportIssue);

            // Handle optional media file upload
            if (mediaFile != null && mediaFile.Length > 0)
            {
                var fileName = Path.GetFileName(mediaFile.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await mediaFile.CopyToAsync(stream);

                reportIssue.MediaPath = "/uploads/" + fileName;
            }

            // Set report date if not already set
            if (reportIssue.DateReported == default)
                reportIssue.DateReported = DateTime.Now;

            _context.Add(reportIssue);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Issue reported successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: ReportIssues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reportIssue = await _context.ReportIssues.FindAsync(id);
            if (reportIssue == null) return NotFound();

            return View(reportIssue);
        }

        // POST: ReportIssues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Location,Category,DateReported,MediaPath")] ReportIssue reportIssue, IFormFile? mediaFile)
        {
            if (id != reportIssue.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle new media file if uploaded
                    if (mediaFile != null && mediaFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(mediaFile.FileName);
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await mediaFile.CopyToAsync(stream);

                        reportIssue.MediaPath = "/uploads/" + fileName;
                    }
                    else
                    {
                        // Preserve existing MediaPath if no new file uploaded
                        var existingReport = await _context.ReportIssues.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                        if (existingReport != null)
                            reportIssue.MediaPath = existingReport.MediaPath;
                    }

                    _context.Update(reportIssue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportIssueExists(reportIssue.Id)) return NotFound();
                    else throw;
                }

                TempData["SuccessMessage"] = "Report updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(reportIssue);
        }

        // GET: ReportIssues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reportIssue = await _context.ReportIssues.FirstOrDefaultAsync(r => r.Id == id);
            if (reportIssue == null) return NotFound();

            return View(reportIssue);
        }

        // POST: ReportIssues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reportIssue = await _context.ReportIssues.FindAsync(id);
            if (reportIssue != null)
            {
                // Delete associated media file if it exists
                if (!string.IsNullOrEmpty(reportIssue.MediaPath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", reportIssue.MediaPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _context.ReportIssues.Remove(reportIssue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReportIssueExists(int id)
        {
            return _context.ReportIssues.Any(r => r.Id == id);
        }
    }
}
