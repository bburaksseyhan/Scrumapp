using System.Collections.Generic;
using scrum_ui.Models;

namespace scrum_ui.ViewModel
{
    public class ParticipantViewModel
    {
        public List<Participants> Participants { get; set; }

        public string Key { get; set; }

        public string Redirect { get; set; }
    }
}