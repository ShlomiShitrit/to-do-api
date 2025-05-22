using Backend.Models;

namespace Backend.Interfaces
{
    public interface ITaskRepository
    {
        public ICollection<TaskModel>? GetTasks();
        public TaskModel? GetTaskById(int id);
        public bool TaskExists(int id);
        public bool CreateTask(TaskModel task);
        public bool UpdateTask(TaskModel task);
        public bool DeleteTask(TaskModel task);
        public bool Save();
    }
}