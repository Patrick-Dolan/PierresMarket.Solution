using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PierresMarket.ViewModels
{
  public class LoginViewModel
  {
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
  }
}