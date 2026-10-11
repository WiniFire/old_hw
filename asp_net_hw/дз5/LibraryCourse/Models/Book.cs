using System.ComponentModel.DataAnnotations;
namespace LibraryCourse.Models;

public class Book
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите название"), StringLength(150)]
    [Display(Name = "Название")]
    public string Title { get; set; } = "";
    [Required(ErrorMessage = "Введите автора"), StringLength(100)]
    [Display(Name = "Автор")]
    public string Author { get; set; } = "";
    [Required(ErrorMessage = "Введите жанр"), StringLength(50)]
    [Display(Name = "Жанр")]
    public string Genre { get; set; } = "";
    [Range(1450, 2100, ErrorMessage = "Год должен быть от 1450 до 2100")]
    [Display(Name = "Год издания")]
    public int Year { get; set; } = DateTime.Today.Year;
    [Range(1, 1000, ErrorMessage = "Количество должно быть от 1 до 1000")]
    [Display(Name = "Количество экземпляров")]
    public int Copies { get; set; } = 1;
    public List<Loan> Loans { get; set; } = [];
}