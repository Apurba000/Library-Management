using LibraryManagement.Core.Models;

namespace LibraryManagement.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    // User-specific operations
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    Task<IEnumerable<User>> GetUsersWithMemberInfoAsync();
} 