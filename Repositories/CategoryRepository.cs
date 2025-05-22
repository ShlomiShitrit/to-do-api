using Backend.Data;
using Backend.Dto;
using Backend.Interfaces;
using Backend.Models;

namespace Backend.Repositories
{
    public class CategoryRepository(DataContext context) : ICategoryRepository
    {
        private readonly DataContext _context = context;

        public Category? GetCategoryById(int id)
        {
            return _context.Categories.Where(e => e.Id == id).FirstOrDefault();

        }
        public ICollection<Category> GetCategories()
        {
            return _context.Categories.OrderBy(c => c.Id).ToList();
        }
        public bool CreateCategory(Category category)
        {
            _context.Add(category);

            return Save();
        }
        public bool UpdateCategory(Category category)
        {
            _context.Update(category);
            return Save();
        }
        public bool DeleteCategory(Category category)
        {
            _context.Remove(category);
            return Save();
        }
        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public ICollection<SubCategory> GetSubCategories(int categoryId)
        {
            return _context.SubCategories.Where(sc => sc.CategoryId == categoryId).OrderBy(sc => sc.Id).ToList();
        }

        public ICollection<TaskModel>? GetTasks(int categoryId)
        {
            return _context.Tasks.Where(t => t.CategoryId == categoryId).OrderBy(t => t.Id).ToList();
        }
    }
}