using Backend.Dto;
using Backend.Models;

namespace Backend.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category? GetCategoryById(int id);
        bool CategoryExists(int id);
        ICollection<SubCategory>? GetSubCategories(int categoryId);
        ICollection<TaskModel>? GetTasks(int categoryId);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}