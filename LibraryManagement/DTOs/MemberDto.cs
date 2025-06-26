using System.ComponentModel.DataAnnotations;
using LibraryManagement.Enums;

namespace LibraryManagement.DTOs;

// DTO for creating a new member
public class CreateMemberDto
{
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
    
    [Required]
    public int UserId { get; set; }
}

// DTO for updating a member
public class UpdateMemberDto
{
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
    
    public MembershipStatus MembershipStatus { get; set; }
}

// DTO for returning member data
public class MemberResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties - using simple DTOs to avoid circular references
    public int ActiveLoansCount { get; set; }
}

// DTO for member with active loans
public class MemberWithLoansDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public MembershipStatus MembershipStatus { get; set; }
    public int ActiveLoansCount { get; set; }
    public List<LoanSummaryDto> ActiveLoans { get; set; } = new();
}

// DTO for member summary (for lists)
public class MemberSummaryDto
{
    public int Id { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
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