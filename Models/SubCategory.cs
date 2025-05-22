namespace Backend.Models
{
    public class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<TaskModel> Tasks { get; set; }
        public string? IconName { get; set; }
        public string? IconType { get; set; }
        public string? ColorHash { get; set; }
    }
}