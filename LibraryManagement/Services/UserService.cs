using LibraryManagement.Models;
using LibraryManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Enums;
using System.Security.Cryptography;
using System.Text;

namespace LibraryManagement.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly LibraryDbContext _context;

    public UserService(IUserRepository userRepository, LibraryDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Business logic: Set default values
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;
            
            // Business logic: Validate username uniqueness
            if (!await _userRepository.IsUsernameUniqueAsync(user.Username))
            {
                throw new InvalidOperationException($"User with username '{user.Username}' already exists.");
            }

            // Business logic: Validate email uniqueness
            if (!await _userRepository.IsEmailUniqueAsync(user.Email))
            {
                throw new InvalidOperationException($"User with email '{user.Email}' already exists.");
            }

            // Business logic: Hash password (in a real app, you'd use proper password hashing)
            // For now, we'll assume the password is already hashed or will be handled by authentication middleware
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new InvalidOperationException("Password hash is required.");
            }

            // Add user to repository
            await _userRepository.AddAsync(user);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            return user;
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            await transaction.RollbackAsync();
            
            switch (pgEx.SqlState)
            {
                case "23505": // Unique constraint violation
                    if (pgEx.ConstraintName?.Contains("Username") == true)
                    {
                        throw new InvalidOperationException($"User with username '{user.Username}' already exists.");
                    }
                    else if (pgEx.ConstraintName?.Contains("Email") == true)
                    {
                        throw new InvalidOperationException($"User with email '{user.Email}' already exists.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Duplicate entry violates unique constraint: {pgEx.ConstraintName}");
                    }
                
                default:
                    throw new InvalidOperationException($"Database error: {pgEx.MessageText}");
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with ID {user.Id} not found.");
            }

            // Business logic: Validate username uniqueness (excluding current user)
            if (!await _userRepository.IsUsernameUniqueAsync(user.Username, user.Id))
            {
                throw new InvalidOperationException($"User with username '{user.Username}' already exists.");
            }

            // Business logic: Validate email uniqueness (excluding current user)
            if (!await _userRepository.IsEmailUniqueAsync(user.Email, user.Id))
            {
                throw new InvalidOperationException($"User with email '{user.Email}' already exists.");
            }

            // Business logic: Update only allowed fields
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Only update password if provided
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash = user.PasswordHash;
            }

            // Update user in repository
            await _userRepository.UpdateAsync(existingUser);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            return existingUser;
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            await transaction.RollbackAsync();
            
            switch (pgEx.SqlState)
            {
                case "23505": // Unique constraint violation
                    if (pgEx.ConstraintName?.Contains("Username") == true)
                    {
                        throw new InvalidOperationException($"User with username '{user.Username}' already exists.");
                    }
                    else if (pgEx.ConstraintName?.Contains("Email") == true)
                    {
                        throw new InvalidOperationException($"User with email '{user.Email}' already exists.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Duplicate entry violates unique constraint: {pgEx.ConstraintName}");
                    }
                
                default:
                    throw new InvalidOperationException($"Database error: {pgEx.MessageText}");
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            // Business logic: Check if user can be deleted (no associated member with active loans)
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == id);
            if (member != null)
            {
                var activeLoans = await _context.Loans.CountAsync(l => l.MemberId == member.Id && l.Status == LoanStatus.Borrowed);
                if (activeLoans > 0)
                {
                    throw new InvalidOperationException($"Cannot delete user with ID {id}. Associated member has {activeLoans} active loan(s).");
                }
            }

            // Business logic: Soft delete instead of hard delete
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            
            // Update user in repository
            await _userRepository.UpdateAsync(user);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _userRepository.GetActiveUsersAsync();
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetUserByUsernameAsync(username);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null)
    {
        return await _userRepository.IsUsernameUniqueAsync(username, excludeId);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        return await _userRepository.IsEmailUniqueAsync(email, excludeId);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        return await _userRepository.GetUsersByRoleAsync(role);
    }

    public async Task<IEnumerable<User>> GetUsersWithMemberInfoAsync()
    {
        return await _userRepository.GetUsersWithMemberInfoAsync();
    }

    // Authentication methods
    public async Task<User?> SignInAsync(string username, string password)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user == null || !user.IsActive)
        {
            return null;
        }

        if (VerifyPassword(password, user.PasswordHash, user.Salt))
        {
            // Update last login date
            user.LastLoginDate = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _context.SaveChangesAsync();
            
            return user;
        }

        return null;
    }

    public async Task<User> SignUpAsync(string username, string email, string password, UserRole role = UserRole.Member)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validate username uniqueness
            if (!await _userRepository.IsUsernameUniqueAsync(username))
            {
                throw new InvalidOperationException($"User with username '{username}' already exists.");
            }

            // Validate email uniqueness
            if (!await _userRepository.IsEmailUniqueAsync(email))
            {
                throw new InvalidOperationException($"User with email '{email}' already exists.");
            }

            // Hash password
            var (passwordHash, salt) = HashPassword(password);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Salt = salt,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return user;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private (string passwordHash, string salt) HashPassword(string password)
    {
        // Generate a random salt
        byte[] saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);

        // Combine password and salt
        string combined = password + salt;
        
        // Hash using SHA256
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        string passwordHash = Convert.ToBase64String(hashBytes);

        return (passwordHash, salt);
    }

    private bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        // Combine password and stored salt
        string combined = password + storedSalt;
        
        // Hash using SHA256
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        string passwordHash = Convert.ToBase64String(hashBytes);

        return passwordHash == storedHash;
    }
} 