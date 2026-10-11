using System.ComponentModel.DataAnnotations;
namespace LibraryCourse.Models;

public class Reader
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите имя читателя"), StringLength(100)]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = "";
    [Required(ErrorMessage = "Введите email"), EmailAddress(ErrorMessage = "Некорректный email"), StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Введите телефон"), StringLength(30)]
    [RegularExpression(@"^[+0-9()\s-]{7,30}$", ErrorMessage = "Некорректный телефон")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = "";
    public List<Loan> Loans { get; set; } = [];
}