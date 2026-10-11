namespace LibraryCourse.Models;

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int ReaderId { get; set; }
    public Reader Reader { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public bool IsOverdue => ReturnedAt == null && DueDate.Date < DateTime.Today;
}