using Backend.Data;
using Backend.Interfaces;
using Backend.Models;

namespace Backend.Repositories
{
    public class TaskRepository(DataContext context) : ITaskRepository
    {
        private readonly DataContext _context = context;

        public bool CreateTask(TaskModel task)
        {
            _context.Add(task);

            return Save();
        }

        public bool DeleteTask(TaskModel task)
        {
            _context.Remove(task);
            return Save();
        }

        public TaskModel? GetTaskById(int id)
        {
            return _context.Tasks.Where(t => t.Id == id).FirstOrDefault();
        }

        public ICollection<TaskModel>? GetTasks()
        {
            return _context.Tasks.OrderBy(t => t.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();

            return saved > 0;
        }

        public bool TaskExists(int id)
        {
            return _context.Tasks.Any(t => t.Id == id);
        }

        public bool UpdateTask(TaskModel task)
        {
            _context.Update(task);
            return Save();
        }
    }
}