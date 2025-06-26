using LibraryManagement.Models;

namespace LibraryManagement.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    Task<IEnumerable<User>> GetUsersWithMemberInfoAsync();
} 