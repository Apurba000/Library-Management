using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryManagement.Enums;

namespace LibraryManagement.Models;

public class Loan
{
    public int Id { get; set; }
    
    [Required]
    public int BookId { get; set; }
    
    [Required]
    public int MemberId { get; set; }
    
    public DateTime LoanDate { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    public DateTime? ReturnDate { get; set; }
    
    public LoanStatus Status { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("BookId")]
    public virtual Book Book { get; set; } = null!;
    
    [ForeignKey("MemberId")]
    public virtual Member Member { get; set; } = null!;
} 