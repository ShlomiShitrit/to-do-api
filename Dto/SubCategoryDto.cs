namespace Backend.Dto
{
    public class SubCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? IconName { get; set; }
        public string? IconType { get; set; }
        public string? ColorHash { get; set; }
        public int? CategoryId { get; set; }
    }
}