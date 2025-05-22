using Microsoft.AspNetCore.Mvc;
using Backend.Dto;
using Backend.Interfaces;
using AutoMapper;
using Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController(
        ISubCategoryRepository subCategoryRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IMapper mapper) : Controller
    {
        private readonly ISubCategoryRepository _subCategoryRepository = subCategoryRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult GetSubCategories()
        {
            var subCategories = _mapper.Map<List<SubCategoryDto>>(_subCategoryRepository.GetSubCategories());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(subCategories);
        }

        [HttpGet("{subCategoryId}")]
        public IActionResult GetSubCategory(int subCategoryId)
        {
            if (!_subCategoryRepository.SubCategoryExists(subCategoryId))
                return NotFound();

            var subCategory = _mapper.Map<SubCategoryDto>(_subCategoryRepository.GetSubCategoryById(subCategoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(subCategory);

        }

        [HttpPost("{categoryId}")]
        [Authorize]
        public IActionResult CreateSubCategory(int categoryId, [FromBody] SubCategoryDto subCategoryCreate)
        {
            if (subCategoryCreate == null)
                return BadRequest(ModelState);

            var subCategory = _subCategoryRepository.GetSubCategories()
                .Where(sc => sc.Name.Trim().ToLower() == subCategoryCreate.Name.TrimEnd().ToLower() && sc.CategoryId == categoryId)
                .FirstOrDefault();

            if (subCategory != null)
            {
                ModelState.AddModelError("SubCategory", "SubCategory already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isCategoryExists = _categoryRepository.CategoryExists(categoryId);
            if (!isCategoryExists)
                return NotFound();

            var subCategoryMap = _mapper.Map<SubCategory>(subCategoryCreate);
            var category = _categoryRepository.GetCategoryById(categoryId);

            if (category == null)
                return NotFound();

            subCategoryMap.Category = category;
            subCategoryMap.CategoryId = categoryId;


            if (!_subCategoryRepository.CreateSubCategory(subCategoryMap))
            {
                ModelState.AddModelError("SubCategory", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpGet("{subCategoryId}/tasks")]
        [Authorize]
        public IActionResult GetTasks(int subCategoryId)
        {
            var currentUser = GetCurrentUser();

            if (!_subCategoryRepository.SubCategoryExists(subCategoryId))
                return NotFound("No subcategory found");

            var subCategory = _subCategoryRepository.GetSubCategoryById(subCategoryId);
            var categoryId = subCategory?.CategoryId;
            var categoryOfUser = _userRepository.GetCategories(currentUser.Id).Where(c => c.Id == categoryId).FirstOrDefault();

            if (categoryOfUser == null)
                return NotFound("User don't have a category with this id");

            var subCategories = _categoryRepository.GetSubCategories(categoryOfUser.Id);

            if (subCategories == null)
                return NotFound("No sub categories found for this user");

            var authSubCategory = subCategories.Where(sc => sc.Id == subCategoryId).FirstOrDefault();

            if (authSubCategory == null)
                return NotFound("This user don't have a sub category with this id");

            var tasks = _mapper.Map<List<TaskDto>>(_subCategoryRepository.GetTasks(authSubCategory.Id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasks);
        }

        [HttpDelete("{subCategoryId}")]
        [Authorize]
        public IActionResult DeleteSubCategory(int subCategoryId)
        {
            if (!subCategoryRepository.SubCategoryExists(subCategoryId))
                return NotFound();

            var subCategoryToDelete = _subCategoryRepository.GetSubCategoryById(subCategoryId);

            if (subCategoryToDelete == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_subCategoryRepository.DeleteSubCategory(subCategoryToDelete))
            {
                ModelState.AddModelError("SubCategory", "Something went wrong deleting subcategory");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpPut("{subCategoryId}")]
        [Authorize]
        public IActionResult UpdateSubCategory(int subCategoryId, [FromQuery] int categoryId, [FromBody] SubCategoryDto updatedSubCatgory)
        {
            if (updatedSubCatgory == null)
                return BadRequest(ModelState);
            if (subCategoryId != updatedSubCatgory.Id)
                return BadRequest(ModelState);
            if (!_subCategoryRepository.SubCategoryExists(subCategoryId))
                return NotFound("Subcategory Doesn't exist");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = _categoryRepository.GetCategoryById(categoryId);

            if (category == null)
                return NotFound();

            var subCategoryMap = _mapper.Map<SubCategory>(updatedSubCatgory);
            subCategoryMap.CategoryId = category.Id;


            if (!_subCategoryRepository.UpdateSubCategory(subCategoryMap))
            {
                ModelState.AddModelError("SubCategory", "Something went wrong updating subcategory");
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