using System.ComponentModel.DataAnnotations;

namespace scrum_ui.ViewModel
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}