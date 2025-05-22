using Backend.Models;

namespace Backend.Interfaces
{
    public interface ISubCategoryRepository
    {
        ICollection<SubCategory> GetSubCategories();
        SubCategory? GetSubCategoryById(int id);
        ICollection<TaskModel>? GetTasks(int subCategoryId);
        bool SubCategoryExists(int id);
        bool CreateSubCategory(SubCategory subCategory);
        bool UpdateSubCategory(SubCategory subCategory);
        bool DeleteSubCategory(SubCategory subCategory);
        bool Save();
    }
}