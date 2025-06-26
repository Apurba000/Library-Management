using LibraryManagement.Models;
using LibraryManagement.DTOs;

namespace LibraryManagement.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            LastLoginDate = user.LastLoginDate,
            CreatedAt = user.CreatedAt
        };
    }

    public static User ToModel(this CreateUserDto dto)
    {
        return new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = string.Empty, // Will be hashed in service
            Salt = string.Empty, // Will be generated in service
            Role = dto.Role,
            IsActive = true
        };
    }

    public static void UpdateModel(this UpdateUserDto dto, User user)
    {
        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Role = dto.Role;
        user.UpdatedAt = DateTime.UtcNow;
    }
} 