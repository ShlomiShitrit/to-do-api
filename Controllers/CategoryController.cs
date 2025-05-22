using AutoMapper;
using Backend.Dto;
using Backend.Interfaces;
using Backend.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
         IMapper mapper
         ) : Controller
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [Authorize(Roles = "Admin")]
        [Category_ValidateCategoryIdFilter]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategoryById(categoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);

        }
        [HttpGet("{categoryId}/subcategories")]
        [Authorize]
        public IActionResult GetSubCategories(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var currentUser = GetCurrentUser();

            var categories = _mapper.Map<List<CategoryDto>>(_userRepository.GetCategories(currentUser.Id));

            var category = categories.Where(c => c.Id == categoryId).FirstOrDefault();

            if (category == null)
                return NotFound("The current user don't have a category with this id");

            var newCategoryId = category.Id;
            var subCategories = _mapper.Map<List<SubCategoryDto>>(_categoryRepository.GetSubCategories(newCategoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(subCategories);
        }

        [HttpGet("{categoryId}/tasks")]
        [Authorize]
        public IActionResult GetTasks(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var currentUser = GetCurrentUser();

            var authCategory = _userRepository.GetCategories(currentUser.Id).Where(c => c.Id == categoryId).FirstOrDefault();

            if (authCategory == null)
                return NotFound("This user don't have a category with this id");

            var tasks = _mapper.Map<List<TaskDto>>(_categoryRepository.GetTasks(authCategory.Id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasks);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
                return NotFound("No user is logged in");

            var currentUserId = currentUser.Id;

            if (categoryCreate == null)
                return BadRequest(ModelState);

            var category = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToLower() == categoryCreate.Name.TrimEnd().ToLower())
                .FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("Category", "Category already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isUserExists = _userRepository.UserExists(currentUserId);
            if (!isUserExists)
                return NotFound();

            var categoryMap = _mapper.Map<Category>(categoryCreate);
            var user = _userRepository.GetUserById(currentUserId);

            if (user == null)
                return NotFound();

            categoryMap.User = user;
            categoryMap.UserId = currentUserId;

            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("Category", "Somthing went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{categoryId}")]
        [Authorize]
        public IActionResult UpdateSubCategory(int categoryId, [FromQuery] int userId,
        [FromBody] CategoryDto updatedCatgory)
        {
            if (updatedCatgory == null)
                return BadRequest(ModelState);
            if (categoryId != updatedCatgory.Id)
                return BadRequest(ModelState);
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound("Category Doesn't exist");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userRepository.GetUserById(userId);

            if (user == null)
                return NotFound("User doesn't exists");

            var categoryMap = _mapper.Map<Category>(updatedCatgory);
            categoryMap.UserId = user.Id;


            if (!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("Category", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete("{categoryId}")]
        [Authorize]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var categoryToDelete = _categoryRepository.GetCategoryById(categoryId);

            if (categoryToDelete == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("Category", "Something went wrong deleting category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        internal User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity ?? null;

            var userClaims = identity?.Claims;

            return new User
            {
                Id = Convert.ToInt32(userClaims?.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value),
                Email = userClaims?.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value ?? "is null",
                FirstName = userClaims?.FirstOrDefault(u => u.Type == ClaimTypes.GivenName)?.Value ?? "is null",
                LastName = userClaims?.FirstOrDefault(u => u.Type == ClaimTypes.Surname)?.Value ?? "is null",
                Role = userClaims?.FirstOrDefault(u => u.Type == ClaimTypes.Role)?.Value ?? "is null",
            };
        }
    }
}