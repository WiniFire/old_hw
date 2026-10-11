using LibraryCourse.Data;
using LibraryCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LibraryCourse.Controllers;

[Authorize]
public class BooksController(LibraryContext db) : Controller
{
    public async Task<IActionResult> Index(string? search, string? genre)
    {
        var query = db.Books.AsNoTracking().Include(b => b.Loans).AsQueryable();
        var books = await query.ToListAsync();
        if (!string.IsNullOrWhiteSpace(search))
            books = books.Where(b => b.Title.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                || b.Author.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        if (!string.IsNullOrWhiteSpace(genre)) books = books.Where(b => b.Genre == genre).ToList();
        ViewBag.Search = search;
        ViewBag.Genre = genre;
        ViewBag.Genres = await db.Books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToListAsync();
        return View(books.OrderBy(b => b.Title).ToList());
    }

    public IActionResult Create() => View("Edit", new Book());

    public async Task<IActionResult> Edit(int id)
    {
        var book = await db.Books.FindAsync(id);
        return book == null ? NotFound() : View(book);
    }

    [HttpPost]
    public async Task<IActionResult> Save(int id, [Bind("Title,Author,Genre,Year,Copies")] Book model)
    {
        model.Id = id;
        if (!ModelState.IsValid) return View("Edit", model);
        await using var transaction = await db.Database.BeginTransactionAsync();
        var book = id == 0 ? new Book() : await db.Books.FindAsync(id);
        if (book == null) return NotFound();
        var active = await db.Loans.CountAsync(l => l.BookId == id && l.ReturnedAt == null);
        if (model.Copies < active)
        {
            ModelState.AddModelError(nameof(Book.Copies), $"Сейчас выдано {active} экз. Уменьшить количество нельзя");
            return View("Edit", model);
        }
        book.Title = model.Title.Trim();
        book.Author = model.Author.Trim();
        book.Genre = model.Genre.Trim();
        book.Year = model.Year;
        book.Copies = model.Copies;
        if (id == 0) db.Books.Add(book);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Message"] = "Книга сохранена";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var book = await db.Books.AsNoTracking().Include(b => b.Loans).SingleOrDefaultAsync(b => b.Id == id);
        return book == null ? NotFound() : View(book);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await db.Books.FindAsync(id);
        if (book == null) return NotFound();
        if (await db.Loans.AnyAsync(l => l.BookId == id))
            TempData["Message"] = "Книга есть в истории выдач, её нельзя удалить";
        else
        {
            db.Books.Remove(book);
            await db.SaveChangesAsync();
            TempData["Message"] = "Книга удалена";
        }
        return RedirectToAction(nameof(Index));
    }
}