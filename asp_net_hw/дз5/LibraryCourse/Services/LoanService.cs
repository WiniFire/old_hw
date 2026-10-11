using LibraryCourse.Data;
using LibraryCourse.Models;
using Microsoft.EntityFrameworkCore;
namespace LibraryCourse.Services;

public class LoanService(LibraryContext db)
{
    public async Task<string?> IssueAsync(IssueModel model)
    {
        if (model.DueDate.Date < DateTime.Today || model.DueDate.Date > DateTime.Today.AddDays(90))
            return "Срок возврата должен быть от сегодняшней даты до 90 дней вперёд";
        await using var transaction = await db.Database.BeginTransactionAsync();
        var book = await db.Books.FindAsync(model.BookId);
        if (book == null || !await db.Readers.AnyAsync(r => r.Id == model.ReaderId))
            return "Книга или читатель не найдены";
        var active = await db.Loans.CountAsync(l => l.BookId == model.BookId && l.ReturnedAt == null);
        if (active >= book.Copies) return "Все экземпляры этой книги уже выданы";
        if (await db.Loans.AnyAsync(l => l.BookId == model.BookId && l.ReaderId == model.ReaderId && l.ReturnedAt == null))
            return "У читателя уже есть эта книга";
        db.Loans.Add(new Loan { BookId = model.BookId, ReaderId = model.ReaderId,
            IssuedAt = DateTime.Today, DueDate = model.DueDate.Date });
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return null;
    }
}