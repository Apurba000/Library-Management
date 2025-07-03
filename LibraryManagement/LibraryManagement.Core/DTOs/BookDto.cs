using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Core.DTOs;

// DTO for creating a new book
public class CreateBookDto
{
    [Required]
    [MaxLength(13)]
    public string ISBN { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Author { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? Publisher { get; set; }
    
    [Range(1800, 2100)]
    public int? PublicationYear { get; set; }
    
    [MaxLength(100)]
    public string? Genre { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Range(1, 1000)]
    public int TotalCopies { get; set; } = 1;
    
    [MaxLength(100)]
    public string? Location { get; set; }
    
    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
}

// DTO for updating a book
public class UpdateBookDto
{
    [Required]
    [MaxLength(13)]
    public string ISBN { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Author { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? Publisher { get; set; }
    
    [Range(1800, 2100)]
    public int? PublicationYear { get; set; }
    
    [MaxLength(100)]
    public string? Genre { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Range(1, 1000)]
    public int TotalCopies { get; set; } = 1;
    
    [MaxLength(100)]
    public string? Location { get; set; }
    
    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
}

// DTO for returning book data (what clients see)
public class BookResponseDto
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public int? PublicationYear { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public string? Location { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// DTO for book availability
public class BookAvailabilityDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int AvailableCopies { get; set; }
    public int TotalCopies { get; set; }
} 