using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using scrum_ui.Models;

namespace scrum_ui.Repository
{
    public interface ISprintLogRepository
    {
         Task<bool> Create(SprintLog log);

        Task<List<IGrouping<string,SprintLog>>> GetAllLogsByGroupId(int groupId);
    }
}