using LibraryManagement.Models;
using LibraryManagement.DTOs;

namespace LibraryManagement.Mappers;

public static class UserMapper
{
    public static UserResponseDto ToDto(this User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static UserWithMemberDto ToWithMemberDto(this User user)
    {
        return new UserWithMemberDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive
        };
    }

    public static User ToModel(this CreateUserDto dto)
    {
        return new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = dto.PasswordHash,
            Role = dto.Role
        };
    }

    public static void UpdateModel(this UpdateUserDto dto, User user)
    {
        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Role = dto.Role;
        
        if (!string.IsNullOrEmpty(dto.PasswordHash))
        {
            user.PasswordHash = dto.PasswordHash;
        }
    }
} 