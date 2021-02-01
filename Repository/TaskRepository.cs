using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using scrum_ui.Context;

namespace scrum_ui.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ScrumContext _context;
        public TaskRepository(ScrumContext context) => _context = context ?? throw new System.ArgumentNullException(nameof(context));

        public async Task<int> CreateTask(Models.Task task)
        {
            _context.Tasks.Add(task);
            var result = await _context.SaveChangesAsync();
            var id = task.Id;

            return id;
        }

        public async Task<IReadOnlyList<Models.Task>> GetTasksByGroupId(int groupId)
        {
            return await _context.Tasks.Where(x => x.GroupId == groupId).ToListAsync();
        }

        public async Task<Models.Task> GetTaskById(int id)
        {
          return await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> RemoveTask(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTaskScore(int id, int score)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
                return false;

            task.Score = score;

            _context.Entry(task).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}