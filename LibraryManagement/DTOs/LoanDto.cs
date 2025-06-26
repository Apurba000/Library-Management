using System.ComponentModel.DataAnnotations;
using LibraryManagement.Enums;

namespace LibraryManagement.DTOs;

// DTO for creating a new loan
public class CreateLoanDto
{
    [Required]
    public int BookId { get; set; }
    
    [Required]
    public int MemberId { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

// DTO for updating a loan (returning a book)
public class UpdateLoanDto
{
    public DateTime? ReturnDate { get; set; }
    
    public LoanStatus Status { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

// DTO for returning loan data
public class LoanResponseDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public BookResponseDto? Book { get; set; }
    public MemberResponseDto? Member { get; set; }
    
    // Calculated properties
    public bool IsOverdue => Status == LoanStatus.Borrowed && DateTime.UtcNow > DueDate;
    public int DaysOverdue => IsOverdue ? (int)(DateTime.UtcNow - DueDate).TotalDays : 0;
    public int DaysRemaining => Status == LoanStatus.Borrowed ? (int)(DueDate - DateTime.UtcNow).TotalDays : 0;
}

// DTO for loan summary (for lists)
public class LoanSummaryDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public bool IsOverdue { get; set; }
    public int DaysOverdue { get; set; }
}

// DTO for loan history
public class LoanHistoryDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public bool WasOverdue { get; set; }
    public int DaysOverdue { get; set; }
}

// DTO for overdue loans
public class OverdueLoanDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string MemberEmail { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
}

// DTO for loan statistics
public class LoanStatisticsDto
{
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public int ReturnedLoans { get; set; }
    public double AverageLoanDuration { get; set; }
    public int TotalOverdueDays { get; set; }
}

// DTO for loan search/filtering
public class LoanSearchDto
{
    public int? BookId { get; set; }
    public int? MemberId { get; set; }
    public LoanStatus? Status { get; set; }
    public DateTime? LoanDateFrom { get; set; }
    public DateTime? LoanDateTo { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public bool? IsOverdue { get; set; }
} 