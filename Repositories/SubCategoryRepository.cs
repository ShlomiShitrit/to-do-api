using Backend.Interfaces;
using Backend.Models;
using Backend.Data;

namespace Backend.Repositories
{
    public class SubCategoryRepository(DataContext context) : ISubCategoryRepository
    {
        private readonly DataContext _context = context;
        public bool CreateSubCategory(SubCategory subCategory)
        {
            _context.Add(subCategory);

            return Save();
        }

        public bool DeleteSubCategory(SubCategory subCategory)
        {
            _context.Remove(subCategory);
            return Save();
        }

        public ICollection<SubCategory> GetSubCategories()
        {
            return _context.SubCategories.OrderBy(c => c.Id).ToList();
        }

        public SubCategory? GetSubCategoryById(int id)
        {
            return _context.SubCategories.Where(e => e.Id == id).FirstOrDefault();
        }

        public ICollection<TaskModel>? GetTasks(int subCategoryId)
        {
            return _context.Tasks.Where(t => t.SubCategoryId == subCategoryId).OrderBy(t => t.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool SubCategoryExists(int id)
        {
            return _context.SubCategories.Any(c => c.Id == id);
        }

        public bool UpdateSubCategory(SubCategory subCategory)
        {
            _context.Update(subCategory);
            return Save();
        }
    }
}