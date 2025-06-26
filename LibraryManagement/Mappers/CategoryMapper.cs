using LibraryManagement.Models;
using LibraryManagement.DTOs;

namespace LibraryManagement.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }

    public static Category ToModel(this CreateCategoryDto dto)
    {
        return new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateModel(this UpdateCategoryDto dto, Category category)
    {
        category.Name = dto.Name;
        category.Description = dto.Description;
        // Note: Category model doesn't have UpdatedAt property
    }
} 