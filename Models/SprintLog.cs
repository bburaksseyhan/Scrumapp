namespace scrum_ui.Models
{
    public class SprintLog
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public string Task { get; set; }

        public int Score { get; set; }

        public string ParticipantName { get; set; }
    }
}