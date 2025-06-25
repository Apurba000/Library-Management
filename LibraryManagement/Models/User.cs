using System.ComponentModel.DataAnnotations;
using LibraryManagement.Enums;

namespace LibraryManagement.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    public string Salt { get; set; } = string.Empty;
    
    public UserRole Role { get; set; } = UserRole.Member;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? LastLoginDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Member? Member { get; set; }
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
} 