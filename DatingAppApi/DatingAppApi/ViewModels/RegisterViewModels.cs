using System.ComponentModel.DataAnnotations;

namespace DatingAppApi.ViewModels
{
    public class RegisterViewModels
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="Debe especificar una password entre 4 y 8 dígitos")]
        public string Password { get; set; }
    }
}