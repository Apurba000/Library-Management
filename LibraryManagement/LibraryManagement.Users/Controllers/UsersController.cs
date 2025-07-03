using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Mappers;
using LibraryManagement.Core.Enums;
using LibraryManagement.Users.Services;

namespace LibraryManagement.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        try
        {
            var users = await _userService.GetActiveUsersAsync();
            var userDtos = users.Select(u => u.ToDto());
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving users", details = ex.Message });
        }
    }

    // GET: api/users/with-member-info
    [HttpGet("with-member-info")]
    public async Task<ActionResult<IEnumerable<UserWithMemberDto>>> GetUsersWithMemberInfo()
    {
        try
        {
            var users = await _userService.GetUsersWithMemberInfoAsync();
            var userDtos = users.Select(u => u.ToWithMemberDto());
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving users with member info", details = ex.Message });
        }
    }

    // GET: api/users/by-role?role=Admin
    [HttpGet("by-role")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsersByRole([FromQuery] string role)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest(new { error = "Role cannot be empty" });
            }

            var users = await _userService.GetUsersByRoleAsync(role);
            var userDtos = users.Select(u => u.ToDto());
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving users by role", details = ex.Message });
        }
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound(new { error = $"User with ID {id} not found" });
            }

            return Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the user", details = ex.Message });
        }
    }

    // GET: api/users/by-username?username=admin
    [HttpGet("by-username")]
    public async Task<ActionResult<UserResponseDto>> GetUserByUsername([FromQuery] string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { error = "Username cannot be empty" });
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            
            if (user == null)
            {
                return NotFound(new { error = $"User with username '{username}' not found" });
            }

            return Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the user", details = ex.Message });
        }
    }

    // GET: api/users/by-email?email=admin@library.com
    [HttpGet("by-email")]
    public async Task<ActionResult<UserResponseDto>> GetUserByEmail([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { error = "Email cannot be empty" });
            }

            var user = await _userService.GetUserByEmailAsync(email);
            
            if (user == null)
            {
                return NotFound(new { error = $"User with email '{email}' not found" });
            }

            return Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the user", details = ex.Message });
        }
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = createUserDto.ToModel();
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the user", details = ex.Message });
        }
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { error = $"User with ID {id} not found" });
            }

            updateUserDto.UpdateModel(existingUser);
            var updatedUser = await _userService.UpdateUserAsync(existingUser);
            return Ok(updatedUser.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating the user", details = ex.Message });
        }
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the user", details = ex.Message });
        }
    }

    // GET: api/users/check-username-unique?username=admin&excludeId=5
    [HttpGet("check-username-unique")]
    public async Task<ActionResult<bool>> IsUsernameUnique([FromQuery] string username, [FromQuery] int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { error = "Username cannot be empty" });
            }

            var isUnique = await _userService.IsUsernameUniqueAsync(username, excludeId);
            return Ok(new { username, excludeId, isUnique });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while checking username uniqueness", details = ex.Message });
        }
    }

    // GET: api/users/check-email-unique?email=admin@library.com&excludeId=5
    [HttpGet("check-email-unique")]
    public async Task<ActionResult<bool>> IsEmailUnique([FromQuery] string email, [FromQuery] int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { error = "Email cannot be empty" });
            }

            var isUnique = await _userService.IsEmailUniqueAsync(email, excludeId);
            return Ok(new { email, excludeId, isUnique });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while checking email uniqueness", details = ex.Message });
        }
    }

    // POST: api/users/signin
    [HttpPost("signin")]
    public async Task<ActionResult<object>> SignIn([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.SignInAsync(loginDto.Username, loginDto.Password);
            
            if (user == null)
            {
                return Unauthorized(new { error = "Invalid username or password" });
            }

            // Return user info without sensitive data
            return Ok(new
            {
                message = "Sign in successful",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    lastLoginDate = user.LastLoginDate
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred during sign in", details = ex.Message });
        }
    }

    // POST: api/users/signup
    [HttpPost("signup")]
    public async Task<ActionResult<object>> SignUp([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate password confirmation
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest(new { error = "Password and confirmation password do not match" });
            }

            var user = await _userService.SignUpAsync(
                registerDto.Username, 
                registerDto.Email, 
                registerDto.Password
            );

            // Return user info without sensitive data
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                message = "User registered successfully",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    createdAt = user.CreatedAt
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred during registration", details = ex.Message });
        }
    }

    // POST: api/users/signup-admin
    [HttpPost("signup-admin")]
    public async Task<ActionResult<object>> SignUpAdmin([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate password confirmation
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest(new { error = "Password and confirmation password do not match" });
            }

            var user = await _userService.SignUpAsync(
                registerDto.Username, 
                registerDto.Email, 
                registerDto.Password,
                UserRole.Admin
            );

            // Return user info without sensitive data
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                message = "Admin user registered successfully",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    createdAt = user.CreatedAt
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred during admin registration", details = ex.Message });
        }
    }
} 