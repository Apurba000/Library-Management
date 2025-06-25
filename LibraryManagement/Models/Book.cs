using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models;

public class Book
{
    public int Id { get; set; }
    
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
    
    public int? PublicationYear { get; set; }
    
    [MaxLength(100)]
    public string? Genre { get; set; }
    
    public string? Description { get; set; }
    
    public int TotalCopies { get; set; } = 1;
    
    public int AvailableCopies { get; set; } = 1;
    
    [MaxLength(100)]
    public string? Location { get; set; }
    
    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }
    
    public int? CategoryId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int? CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }
    
    [ForeignKey("CreatedBy")]
    public virtual User? CreatedByUser { get; set; }
    
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
} 