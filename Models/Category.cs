namespace Backend.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }
        public ICollection<TaskModel> Tasks { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string? IconName { get; set; }
        public string? IconType { get; set; }
        public string? ColorHash { get; set; }

    }
}