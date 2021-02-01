using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using scrum_ui.Context;
using scrum_ui.Models;

namespace scrum_ui.Repository
{
    public class SprintLogRepository : ISprintLogRepository
    {
        private readonly ScrumContext _context;
    
        public SprintLogRepository(ScrumContext context) => _context = context ?? throw new System.ArgumentNullException(nameof(context));

        public async Task<bool> Create(SprintLog log)
        {
            var isExist = _context.SprintLogs.FirstOrDefaultAsync(x => x.Task == log.Task);
            if(isExist != null)
                return false;
                
            _context.Add(log);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<IGrouping<string,SprintLog>>> GetAllLogsByGroupId(int groupId)
        {
           return await _context.SprintLogs.Where(x => x.GroupId == groupId).GroupBy(x => x.ParticipantName).ToListAsync();
        }
    }
}