using System.Threading.Tasks;
using scrum_ui.Models;

namespace scrum_ui.Repository
{
    public interface IUserRepository
    {
         Task<bool> Register(User user);

         Task<User> GetUser(string email, string password);

         Task<User> GetUser(string email);
    }
}