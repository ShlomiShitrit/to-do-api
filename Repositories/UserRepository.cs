using Backend.Interfaces;
using Backend.Models;
using Backend.Data;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Repositories
{
    public class UserRepository(DataContext context) : IUserRepository
    {
        private readonly DataContext _context = context;
        public bool CreateUser(User user)
        {
            _context.Add(user);

            return Save();
        }

        public bool DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public ICollection<Category> GetCategories(int userId)
        {
            return _context.Categories.Where(c => c.UserId == userId).OrderBy(c => c.Id).ToList();
        }

        public ICollection<TaskModel> GetTasks(int userId)
        {
            var tasks = new List<TaskModel>();
            var categoryTasks = _context.Categories.Where(c => c.UserId == userId)
                                                  .SelectMany(c => c.Tasks)
                                                  .ToList();
            var subcategoryTasks = _context.SubCategories.Where(s => s.Category.UserId == userId)
                                                        .SelectMany(s => s.Tasks)
                                                        .ToList();
            tasks.AddRange(categoryTasks);
            tasks.AddRange(subcategoryTasks);
            return tasks;
        }

        public User? GetUserByEmailAndPassword(string email, string password)
        {
            return _context.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
        }

        public User? GetUserById(int id)
        {
            return _context.Users.Where(u => u.Id == id).FirstOrDefault();
        }

        public ICollection<User>? GetUsers()
        {
            return _context.Users.OrderBy(u => u.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();

            return saved > 0;
        }

        public bool UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool UserExists(int id)
        {
            return _context.Users.Any(u => u.Id == id);
        }

    }

}
