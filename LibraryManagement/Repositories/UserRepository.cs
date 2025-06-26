using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Enums;

namespace LibraryManagement.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Username.ToLower() == username.ToLower() && u.IsActive);
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        if (Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            return await _dbSet
                .Where(u => u.Role == userRole && u.IsActive)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        return Enumerable.Empty<User>();
    }

    public async Task<IEnumerable<User>> GetUsersWithMemberInfoAsync()
    {
        return await _dbSet
            .Include(u => u.Member)
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }
} 