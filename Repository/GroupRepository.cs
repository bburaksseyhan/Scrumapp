using System.Threading.Tasks;
using scrum_ui.Context;
using scrum_ui.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;
namespace scrum_ui.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ScrumContext _context;
        private readonly IParticipantRepository _participantRepository;
        public GroupRepository(ScrumContext context, IParticipantRepository participantRepository)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _participantRepository = participantRepository ?? throw new System.ArgumentNullException(nameof(participantRepository));
        }

        public async Task<bool> Create(Group group)
        {  
            var result = await _context.Groups.FirstOrDefaultAsync(x => x.Name == group.Name); // aynı gruptan 1 den fazla oluşturulamaz.
            if (result != null)
                return false;

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            var participants = group.Participants.Split(";");
            
            foreach (var participant in participants)
            {
                _context.Participants.Add(new Participants() { GroupId = group.Id, Name = participant, IsOwner = false, Image = GetRandomImagePath()});
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Group>> GetGroupAsync(int userId)
        {
            return await _context.Groups.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<Group> GetSingleGroup(int GroupId){
            return await _context.Groups.FirstOrDefaultAsync(x => x.Id == GroupId);
        }

        public async Task<bool> Delete(int groupId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);

            if (group == null || group.IsActive)
                return false;

            _context.Remove(group);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Close(int groupId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);

            if (group == null)
                return false;

            group.IsActive = false;
            _context.Entry(group).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Group> GetSingleGroupByKeyAsync(string key)
        {
            return await _context.Groups.FirstOrDefaultAsync(x => x.Key == key);
        }

        private static string GetRandomImagePath()
        {
            var images = new List<string>();

            images.Add("/img/donatello.png");
            images.Add("/img/leonardo.png");
            images.Add("/img/raphael.png");
            images.Add("/img/michaelangelo.png");
            images.Add("/img/puffer-fish.png");
            images.Add("/img/bear.png");
            images.Add("/img/cat.png");
            images.Add("/img/dog.png");
            images.Add("/img/kitty.png");

            var random = new Random();
            var id = random.Next(0, images.Count());

            return images[id];
        }
    }
}