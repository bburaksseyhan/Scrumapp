using System;
using System.ComponentModel.DataAnnotations;

namespace scrum_ui.ViewModel
{
    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }

        public DateTime CreateDate {get;set;} = DateTime.Now;
    }
}