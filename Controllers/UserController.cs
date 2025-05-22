using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Interfaces;
using Backend.Filters.ActionFilters;
using Backend.Dto;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(
        IUserRepository userRepository,
        IMapper mapper
    ) : Controller
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers()
        {
            var users = _mapper.Map<List<UserDto>>(_userRepository.GetUsers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(users);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _mapper.Map<UserDto>(_userRepository.GetUserById(userId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }

        [HttpPost]
        [User_ValidateUserSignupFilter]
        public IActionResult CreateUser([FromBody] UserDto userCreate)
        {
            if (userCreate == null)
                return BadRequest(ModelState);

            var user = _userRepository.GetUsers()?
                .Where(u => u.Email.Trim().ToLower() == userCreate.Email.TrimEnd().ToLower())
                .FirstOrDefault();

            if (user != null)
            {
                ModelState.AddModelError("User", "User with this email is already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userMap = _mapper.Map<User>(userCreate);

            if (!_userRepository.CreateUser(userMap))
            {
                ModelState.AddModelError("User", "Somthing went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpGet("categories")]
        [Authorize]
        public IActionResult GetCategoriesOfUser()
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
                return NotFound("No user is logged in");

            if (!_userRepository.UserExists(currentUser.Id))
                return NotFound();

            var categories = _mapper.Map<List<CategoryDto>>(_userRepository.GetCategories(currentUser.Id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(categories);
        }

        [HttpGet("userinfo")]
        [Authorize]
        public IActionResult GetCurrentUserInfo()
        {
            var user = GetCurrentUser();
            if (user == null)
                return NotFound("No user is logged in");
            var userFromDb = _userRepository.GetUserById(user.Id);

            if (userFromDb == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(userFromDb);
        }

        [HttpGet("tasks")]
        [Authorize]
        public IActionResult GetTasksOfUser()
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
                return NotFound("No user is logged in");

            if (!_userRepository.UserExists(currentUser.Id))
                return NotFound();

            var tasks = _mapper.Map<List<TaskDto>>(_userRepository.GetTasks(currentUser.Id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tasks);
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