using LibraryManagement.Models;
using LibraryManagement.DTOs;
using LibraryManagement.Enums;

namespace LibraryManagement.Mappers;

public static class LoanMapper
{
    public static LoanResponseDto ToDto(this Loan loan)
    {
        return new LoanResponseDto
        {
            Id = loan.Id,
            BookId = loan.BookId,
            MemberId = loan.MemberId,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            Notes = loan.Notes,
            CreatedAt = loan.CreatedAt,
            UpdatedAt = loan.UpdatedAt,
            Book = loan.Book?.ToDto(),
            Member = loan.Member?.ToDto()
        };
    }

    public static LoanSummaryDto ToSummaryDto(this Loan loan)
    {
        var isOverdue = loan.Status == LoanStatus.Borrowed && DateTime.UtcNow > loan.DueDate;
        var daysOverdue = isOverdue ? (int)(DateTime.UtcNow - loan.DueDate).TotalDays : 0;

        return new LoanSummaryDto
        {
            Id = loan.Id,
            BookTitle = loan.Book?.Title ?? "Unknown Book",
            MemberName = loan.Member != null ? $"{loan.Member.FirstName} {loan.Member.LastName}" : "Unknown Member",
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            IsOverdue = isOverdue,
            DaysOverdue = daysOverdue
        };
    }

    public static LoanHistoryDto ToHistoryDto(this Loan loan)
    {
        var wasOverdue = loan.Status == LoanStatus.Returned && loan.ReturnDate.HasValue && loan.ReturnDate > loan.DueDate;
        var daysOverdue = wasOverdue ? (int)(loan.ReturnDate!.Value - loan.DueDate).TotalDays : 0;

        return new LoanHistoryDto
        {
            Id = loan.Id,
            BookTitle = loan.Book?.Title ?? "Unknown Book",
            BookAuthor = loan.Book?.Author ?? "Unknown Author",
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            WasOverdue = wasOverdue,
            DaysOverdue = daysOverdue
        };
    }

    public static OverdueLoanDto ToOverdueDto(this Loan loan)
    {
        var daysOverdue = (int)(DateTime.UtcNow - loan.DueDate).TotalDays;

        return new OverdueLoanDto
        {
            Id = loan.Id,
            BookTitle = loan.Book?.Title ?? "Unknown Book",
            MemberName = loan.Member != null ? $"{loan.Member.FirstName} {loan.Member.LastName}" : "Unknown Member",
            MemberEmail = loan.Member?.User?.Email ?? "No Email",
            DueDate = loan.DueDate,
            DaysOverdue = daysOverdue
        };
    }

    public static Loan ToModel(this CreateLoanDto dto)
    {
        return new Loan
        {
            BookId = dto.BookId,
            MemberId = dto.MemberId,
            LoanDate = DateTime.UtcNow,
            DueDate = dto.DueDate,
            Status = LoanStatus.Borrowed,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateModel(this UpdateLoanDto dto, Loan loan)
    {
        if (dto.ReturnDate.HasValue)
        {
            loan.ReturnDate = dto.ReturnDate.Value;
        }
        
        loan.Status = dto.Status;
        loan.Notes = dto.Notes;
        loan.UpdatedAt = DateTime.UtcNow;
    }
} 