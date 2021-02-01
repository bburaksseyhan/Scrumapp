namespace scrum_ui.Models
{
    public class Session
    {
        public string Email { get; set; }

        public string Key { get; set; }

        public bool isOwner { get; set; }

        public string Image { get; set; }

        public Group Group { get; set; } = new Group();
    }
}