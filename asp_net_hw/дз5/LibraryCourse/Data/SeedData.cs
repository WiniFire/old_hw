using LibraryCourse.Models;
using Microsoft.AspNetCore.Identity;
namespace LibraryCourse.Data;

public static class SeedData
{
    public static void Initialize(LibraryContext db, IConfiguration configuration)
    {
        if (!db.Librarians.Any())
        {
            using var transaction = db.Database.BeginTransaction();
            var user = new Librarian { Login = configuration["DemoAccount:Login"] ?? "admin" };
            user.PasswordHash = new PasswordHasher<Librarian>().HashPassword(user,
                configuration["DemoAccount:Password"] ?? throw new InvalidOperationException("Не задан пароль"));
            db.Librarians.Add(user);
            db.Books.AddRange(
                new Book { Title = "Маленький принц", Author = "Антуан де Сент-Экзюпери", Genre = "Повесть", Year = 1943, Copies = 3 },
                new Book { Title = "Хоббит", Author = "Джон Толкин", Genre = "Фэнтези", Year = 1937, Copies = 2 },
                new Book { Title = "1984", Author = "Джордж Оруэлл", Genre = "Роман", Year = 1949, Copies = 2 },
                new Book { Title = "Чистый код", Author = "Роберт Мартин", Genre = "Программирование", Year = 2008, Copies = 1 },
                new Book { Title = "Кобзарь", Author = "Тарас Шевченко", Genre = "Поэзия", Year = 1840, Copies = 3 });
            db.Readers.AddRange(
                new Reader { FullName = "Алексей Коваленко", Email = "alex@example.com", Phone = "+380671234567" },
                new Reader { FullName = "Мария Бондарь", Email = "maria@example.com", Phone = "+380681234567" });
            db.SaveChanges();
            db.Loans.Add(new Loan { BookId = db.Books.Single(b => b.Title == "Хоббит").Id,
                ReaderId = db.Readers.Single(r => r.Email == "alex@example.com").Id,
                IssuedAt = DateTime.Today.AddDays(-18), DueDate = DateTime.Today.AddDays(-4) });
            db.SaveChanges();
            transaction.Commit();
        }
    }
}
