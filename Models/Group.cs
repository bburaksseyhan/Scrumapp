using System;

namespace scrum_ui.Models
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; } //task name

        public bool IsActive { get; set; }

        public string Key { get; set; }

        public int UserId { get; set; } //Owner ID

        public string Participants { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public string Redirect { get; set; }
    }
}