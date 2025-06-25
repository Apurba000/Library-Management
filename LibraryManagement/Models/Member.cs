using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryManagement.Enums;

namespace LibraryManagement.Models;

public class Member
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string MemberNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public DateTime MembershipDate { get; set; }
    
    public DateTime? MembershipExpiryDate { get; set; }
    
    public MembershipStatus MembershipStatus { get; set; } = MembershipStatus.Active;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
} 