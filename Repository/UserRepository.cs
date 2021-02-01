using System.Threading.Tasks;
using scrum_ui.Context;
using scrum_ui.Models;
using Microsoft.EntityFrameworkCore;

namespace scrum_ui.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ScrumContext _context;
        public UserRepository(ScrumContext context) => _context = context ?? throw new System.ArgumentNullException(nameof(context));

        public async Task<User> GetUser(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        public async Task<User> GetUser(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> Register(User user)
        {
            if (await GetUser(user.Email, user.Password) != null)
                return false;

            _context.Users.Add(user);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}