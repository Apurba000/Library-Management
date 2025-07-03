using LibraryManagement.Core.Models;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Mappers;

public static class CategoryMapper
{
    public static CategoryResponseDto ToDto(this Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            BookCount = category.Books?.Count(b => b.IsActive) ?? 0
        };
    }

    public static CategoryWithBookCountDto ToBookCountDto(this Category category)
    {
        return new CategoryWithBookCountDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            BookCount = category.Books?.Count(b => b.IsActive) ?? 0,
            IsActive = category.IsActive
        };
    }

    public static Category ToModel(this CreateCategoryDto dto)
    {
        return new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static void UpdateModel(this UpdateCategoryDto dto, Category category)
    {
        category.Name = dto.Name;
        category.Description = dto.Description;
    }
} 