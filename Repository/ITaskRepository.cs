using System.Collections.Generic;
using System.Threading.Tasks;

namespace scrum_ui.Repository
{
    public interface ITaskRepository
    {
         Task<int> CreateTask(scrum_ui.Models.Task task);

         Task<IReadOnlyList<Models.Task>> GetTasksByGroupId(int groupId);
         Task<bool> RemoveTask(int id);

         Task<bool> UpdateTaskScore(int id, int score);

         Task<Models.Task> GetTaskById(int id);
    }
}