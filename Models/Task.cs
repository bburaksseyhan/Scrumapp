namespace scrum_ui.Models
{
    public class Task
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? Score { get; set; }

        public int UserId { get; set; }

        public int GroupId { get; set; }
    }
}