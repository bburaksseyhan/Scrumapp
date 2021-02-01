using System.Collections.Generic;
using System.Threading.Tasks;
using scrum_ui.Models;

namespace scrum_ui.Repository
{
    public interface IGroupRepository
    {
        Task<bool> Create(Group group);
        Task<List<Group>> GetGroupAsync(int userId);
        Task<bool> Delete(int groupId);
        Task<Group> GetSingleGroupByKeyAsync(string key);
        Task<bool> Close(int groupId);

        Task<Group> GetSingleGroup(int GroupId);
    }
}