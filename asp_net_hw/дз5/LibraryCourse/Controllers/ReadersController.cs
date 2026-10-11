using LibraryCourse.Data;
using LibraryCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LibraryCourse.Controllers;

[Authorize]
public class ReadersController(LibraryContext db) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var readers = await db.Readers.AsNoTracking().Include(r => r.Loans).ToListAsync();
        if (!string.IsNullOrWhiteSpace(search))
            readers = readers.Where(r => r.FullName.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                || r.Email.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        ViewBag.Search = search;
        return View(readers.OrderBy(r => r.FullName).ToList());
    }

    public IActionResult Create() => View("Edit", new Reader());

    public async Task<IActionResult> Edit(int id)
    {
        var reader = await db.Readers.FindAsync(id);
        return reader == null ? NotFound() : View(reader);
    }

    [HttpPost]
    public async Task<IActionResult> Save(int id, [Bind("FullName,Email,Phone")] Reader model)
    {
        model.Id = id;
        model.Email = (model.Email ?? "").Trim().ToLowerInvariant();
        if (await db.Readers.AnyAsync(r => r.Email == model.Email && r.Id != id))
            ModelState.AddModelError(nameof(Reader.Email), "Читатель с таким email уже зарегистрирован");
        if (!ModelState.IsValid) return View("Edit", model);
        var reader = id == 0 ? new Reader() : await db.Readers.FindAsync(id);
        if (reader == null) return NotFound();
        reader.FullName = model.FullName.Trim();
        reader.Email = model.Email;
        reader.Phone = model.Phone.Trim();
        if (id == 0) db.Readers.Add(reader);
        await db.SaveChangesAsync();
        TempData["Message"] = "Читатель сохранён";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var reader = await db.Readers.AsNoTracking().Include(r => r.Loans).SingleOrDefaultAsync(r => r.Id == id);
        return reader == null ? NotFound() : View(reader);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reader = await db.Readers.FindAsync(id);
        if (reader == null) return NotFound();
        if (await db.Loans.AnyAsync(l => l.ReaderId == id))
            TempData["Message"] = "У читателя есть история выдач, его нельзя удалить";
        else
        {
            db.Readers.Remove(reader);
            await db.SaveChangesAsync();
            TempData["Message"] = "Читатель удалён";
        }
        return RedirectToAction(nameof(Index));
    }
}
