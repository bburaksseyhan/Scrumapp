using System;

namespace scrum_ui.ViewModel
{
    public class GroupViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Key { get; set; }

        public int Owner { get; set; }

        public string Participants { get; set; }

        public string Email { get; set; }

        public string Redirect {get; set;}
        
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}