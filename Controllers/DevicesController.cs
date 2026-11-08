using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers;

[Route("devices")]
public class DevicesController : Controller
{
    private readonly AppDbContext _db;
    public DevicesController(AppDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, bool availableOnly)
    {
        var query = _db.Devices.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.Name.Contains(search));
        if (availableOnly)
            query = query.Where(d => !d.IsOnLoan);

        var devices = await query.OrderBy(d => d.AssetTag).ToListAsync();

        ViewBag.CountText = $"{devices.Count} device{(devices.Count == 1 ? "" : "s")}";
        ViewBag.Search = search;
        ViewBag.AvailableOnly = availableOnly;
        return View(devices);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var device = await _db.Devices.FindAsync(id);
        return device == null ? NotFound() : View(device);
    }

    [HttpGet("add")]
    public IActionResult Add() => View(new Device());

    [HttpPost("add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind("AssetTag,Name,Category,PurchaseYear,IsOnLoan")] Device device)
    {
        if (await _db.Devices.AnyAsync(d => d.AssetTag == device.AssetTag))
            ModelState.AddModelError(nameof(Device.AssetTag), "Another device already has that asset tag");
        if (!ModelState.IsValid) return View(device);

        _db.Devices.Add(device);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(Device.AssetTag), "Another device already has that asset tag");
            return View(device);
        }
        TempData["Message"] = $"{device.AssetTag} {device.Name} was added";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var device = await _db.Devices.FindAsync(id);
        return device == null ? NotFound() : View(device);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("AssetTag,Name,Category,PurchaseYear,IsOnLoan")] Device device)
    {
        var existing = await _db.Devices.FindAsync(id);
        if (existing == null) return NotFound();
        device.Id = id;

        if (await _db.Devices.AnyAsync(d => d.AssetTag == device.AssetTag && d.Id != id))
            ModelState.AddModelError(nameof(Device.AssetTag), "Another device already has that asset tag");
        if (!ModelState.IsValid) return View(device);

        existing.AssetTag = device.AssetTag;
        existing.Name = device.Name;
        existing.Category = device.Category;
        existing.PurchaseYear = device.PurchaseYear;
        existing.IsOnLoan = device.IsOnLoan;
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(Device.AssetTag), "Another device already has that asset tag");
            return View(device);
        }
        TempData["Message"] = $"{existing.AssetTag} {existing.Name} was updated";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var device = await _db.Devices.FindAsync(id);
        return device == null ? NotFound() : View(device);
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var device = await _db.Devices.FindAsync(id);
        if (device == null)
        {
            TempData["Message"] = "That device was already deleted";
            return RedirectToAction(nameof(Index));
        }
        _db.Devices.Remove(device);
        try
        {
            await _db.SaveChangesAsync();
            TempData["Message"] = $"{device.AssetTag} {device.Name} was deleted";
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["Message"] = "That device was already deleted";
        }
        return RedirectToAction(nameof(Index));
    }
}