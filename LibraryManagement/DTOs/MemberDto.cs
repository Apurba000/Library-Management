using System.ComponentModel.DataAnnotations;
using LibraryManagement.Enums;

namespace LibraryManagement.DTOs;

// DTO for creating a new member
public class CreateMemberDto
{
    [Required]
    [MaxLength(20)]
    public string MemberNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    public DateTime DateOfBirth { get; set; }
    
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
    
    public DateTime MembershipStartDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? MembershipEndDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

// DTO for updating a member
public class UpdateMemberDto
{
    [Required]
    [MaxLength(20)]
    public string MemberNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    public DateTime DateOfBirth { get; set; }
    
    public MembershipStatus Status { get; set; }
    
    public DateTime MembershipStartDate { get; set; }
    
    public DateTime? MembershipEndDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

// DTO for returning member data
public class MemberResponseDto
{
    public int Id { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public MembershipStatus Status { get; set; }
    public DateTime MembershipStartDate { get; set; }
    public DateTime? MembershipEndDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public UserDto? User { get; set; }
    public int ActiveLoansCount { get; set; }
    public int TotalLoansCount { get; set; }
}

// DTO for member summary (for lists)
public class MemberSummaryDto
{
    public int Id { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public MembershipStatus Status { get; set; }
    public int ActiveLoansCount { get; set; }
}

// DTO for member search/filtering
public class MemberSearchDto
{
    public string? SearchTerm { get; set; }
    public MembershipStatus? Status { get; set; }
    public bool? HasActiveLoans { get; set; }
    public DateTime? MembershipStartDateFrom { get; set; }
    public DateTime? MembershipStartDateTo { get; set; }
} 