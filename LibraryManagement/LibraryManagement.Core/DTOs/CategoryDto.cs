using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

// DTO for creating a new category
public class CreateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
}

// DTO for updating a category
public class UpdateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
}

// DTO for returning category data
public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BookCount { get; set; }
}

// DTO for category with book count
public class CategoryWithBookCountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int BookCount { get; set; }
    public bool IsActive { get; set; }
}

// DTO for category summary (for dropdowns)
public class CategorySummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BookCount { get; set; }
}

// DTO for category statistics
public class CategoryStatisticsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int BorrowedBooks { get; set; }
    public double UtilizationRate { get; set; } // Percentage of books borrowed
    public int TotalLoans { get; set; }
    public double AverageLoanDuration { get; set; }
} 