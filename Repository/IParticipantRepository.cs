using System.Collections.Generic;
using System.Threading.Tasks;
using scrum_ui.Models;

namespace scrum_ui.Repository
{
    public interface IParticipantRepository
    {
        Task<bool> Create(Participants participant);

        Task<List<Participants>> GetParticipants(int GroupId);

        Task<Participants> GetParticipant(string email);
    }
}