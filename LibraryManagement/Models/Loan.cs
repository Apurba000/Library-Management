using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models;

public class Loan
{
    public int Id { get; set; }
    
    [Required]
    public int BookId { get; set; }
    
    [Required]
    public int MemberId { get; set; }
    
    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime DueDate { get; set; }
    
    public DateTime? ReturnDate { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Borrowed";
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("BookId")]
    public virtual Book Book { get; set; } = null!;
    
    [ForeignKey("MemberId")]
    public virtual Member Member { get; set; } = null!;
} 