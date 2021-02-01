using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using scrum_ui.Context;
using scrum_ui.Models;

namespace scrum_ui.Repository
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ScrumContext _context;
    
        public ParticipantRepository(ScrumContext context) => _context = context ?? throw new System.ArgumentNullException(nameof(context));
        
        public async Task<bool> Create(Participants participant)
        {
            _context.Add(participant);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Participants> GetParticipant(string email)
        {
            return await  _context.Participants.FirstOrDefaultAsync(x => x.Name == email);
        }
        
        public async Task<List<Participants>> GetParticipants(int GroupId)
        {
            return await _context.Participants.Where(x=>x.GroupId == GroupId).ToListAsync();
        }
    }
}