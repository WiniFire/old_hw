namespace LibraryCourse.Models;

public class Librarian
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}