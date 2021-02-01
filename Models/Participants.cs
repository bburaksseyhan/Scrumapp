namespace scrum_ui.Models
{
    public class Participants
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? GroupId { get; set; }

        public bool IsOwner { get; set; }

        public string Image { get; set; }
    }
}