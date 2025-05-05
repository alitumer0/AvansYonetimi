using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.Core.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; }
    }
}