using System.ComponentModel.DataAnnotations;
namespace LibraryCourse.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Введите логин"), Display(Name = "Логин")]
    public string Login { get; set; } = "";
    [Required(ErrorMessage = "Введите пароль"), DataType(DataType.Password), Display(Name = "Пароль")]
    public string Password { get; set; } = "";
}

public class IssueModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Выберите книгу"), Display(Name = "Книга")]
    public int BookId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Выберите читателя"), Display(Name = "Читатель")]
    public int ReaderId { get; set; }
    [Required, DataType(DataType.Date), Display(Name = "Вернуть до")]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(14);
}

public class DashboardModel
{
    public int Books { get; set; }
    public int Readers { get; set; }
    public int ActiveLoans { get; set; }
    public int Overdue { get; set; }
    public List<Loan> RecentLoans { get; set; } = [];
}