using LibraryCourse.Data;
using LibraryCourse.Models;
using LibraryCourse.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace LibraryCourse.Controllers;

[Authorize]
public class LoansController(LibraryContext db, LoanService service) : Controller
{
    public async Task<IActionResult> Index(string status = "active")
    {
        var query = db.Loans.AsNoTracking().Include(l => l.Book).Include(l => l.Reader).AsQueryable();
        query = status switch
        {
            "active" => query.Where(l => l.ReturnedAt == null),
            "overdue" => query.Where(l => l.ReturnedAt == null && l.DueDate < DateTime.Today),
            "returned" => query.Where(l => l.ReturnedAt != null),
            _ => query
        };
        ViewBag.Status = status;
        return View(await query.OrderByDescending(l => l.Id).ToListAsync());
    }

    public async Task<IActionResult> Create(int? bookId)
    {
        await LoadLists();
        return View(new IssueModel { BookId = bookId ?? 0 });
    }

    [HttpPost]
    public async Task<IActionResult> Create(IssueModel model)
    {
        if (ModelState.IsValid)
        {
            var error = await service.IssueAsync(model);
            if (error == null)
            {
                TempData["Message"] = "Книга выдана читателю";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", error);
        }
        await LoadLists();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Return(int id)
    {
        var changed = await db.Loans.Where(l => l.Id == id && l.ReturnedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(l => l.ReturnedAt, DateTime.Today));
        TempData["Message"] = changed == 0 ? "Выдача не найдена или книга уже возвращена" : "Возврат оформлен";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLists()
    {
        var books = await db.Books.AsNoTracking().Include(b => b.Loans).OrderBy(b => b.Title).ToListAsync();
        ViewBag.Books = new SelectList(books.Where(b => b.Copies > b.Loans.Count(l => l.ReturnedAt == null))
            .Select(b => new { b.Id, Name = $"{b.Title} — {b.Author}" }), "Id", "Name");
        ViewBag.Readers = new SelectList(await db.Readers.OrderBy(r => r.FullName).ToListAsync(), "Id", "FullName");
    }
}