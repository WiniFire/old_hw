using LibraryCourse.Data;
using LibraryCourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LibraryCourse.Controllers;

[Authorize]
public class HomeController(LibraryContext db) : Controller
{
    public async Task<IActionResult> Index() => View(new DashboardModel
    {
        Books = await db.Books.CountAsync(),
        Readers = await db.Readers.CountAsync(),
        ActiveLoans = await db.Loans.CountAsync(l => l.ReturnedAt == null),
        Overdue = await db.Loans.CountAsync(l => l.ReturnedAt == null && l.DueDate < DateTime.Today),
        RecentLoans = await db.Loans.AsNoTracking().Include(l => l.Book).Include(l => l.Reader)
            .OrderByDescending(l => l.Id).Take(5).ToListAsync()
    });

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}