using System;

namespace scrum_ui.ViewModel
{
    public class UserViewModel
    {
         public int Id { get; set; }

        public string Email { get; set; }

        public bool IsVerified { get; set; }

        public string Password {get; set;}
        
        public int? GroupId {get; set;}

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}