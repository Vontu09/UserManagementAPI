using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all users
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Retrieving all users");
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving users" });
            }
        }

        /// <summary>
        /// Retrieve a specific user by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving user with ID: {UserId}", id);
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound(new { message = $"User with ID {id} not found" });
                }
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid input for user ID {UserId}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the user" });
            }
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for user creation");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creating new user with email: {Email}", user.Email);
                var createdUser = await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning("Null argument error: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation error creating user: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Conflict creating user: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating user");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while creating the user" });
            }
        }

        /// <summary>
        /// Update an existing user's details
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for user update");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Updating user with ID: {UserId}", id);
                var updatedUser = await _userService.UpdateUserAsync(id, user);
                if (updatedUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update", id);
                    return NotFound(new { message = $"User with ID {id} not found" });
                }
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation error updating user: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
           
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Conflict updating user: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating user with ID {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while updating the user" });
            }
        }

        /// <summary>
        /// Remove a user by ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user with ID: {UserId}", id);
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                    return NotFound(new { message = $"User with ID {id} not found" });
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid input for user deletion: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting user with ID {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while deleting the user" });
            }
        }
    }
}