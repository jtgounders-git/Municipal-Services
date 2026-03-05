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
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ServiceRequests.ToListAsync();
            return View(requests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public IActionResult Create() => View();

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest, IFormFile? mediaFile)
        {
            if (!ModelState.IsValid) return View(serviceRequest);

            serviceRequest.Status = RequestStatus.Waiting;
            serviceRequest.CreatedAt = DateTime.Now;

            if (mediaFile != null && mediaFile.Length > 0)
            {
                var fileName = Path.GetFileName(mediaFile.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mediaFile.CopyToAsync(stream);
                }
                serviceRequest.MediaPath = "/uploads/" + fileName;
            }

            _context.Add(serviceRequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Service request created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Location,Category,Status,Priority,CreatedAt,MediaPath")] ServiceRequest serviceRequest, IFormFile? mediaFile)
        {
            if (id != serviceRequest.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (mediaFile != null && mediaFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(mediaFile.FileName);
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await mediaFile.CopyToAsync(stream);
                        }
                        serviceRequest.MediaPath = "/uploads/" + fileName;
                    }
                    else
                    {
                        var existing = await _context.ServiceRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                        if (existing != null)
                            serviceRequest.MediaPath = existing.MediaPath;
                    }

                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.Id))
                        return NotFound();
                    else
                        throw;
                }

                TempData["SuccessMessage"] = "Service request updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest != null)
            {
                if (!string.IsNullOrEmpty(serviceRequest.MediaPath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serviceRequest.MediaPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(Guid id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }
    }
}
