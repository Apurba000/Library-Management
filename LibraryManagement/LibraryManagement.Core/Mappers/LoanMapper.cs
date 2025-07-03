using LibraryManagement.Core.Models;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Enums;

namespace LibraryManagement.Core.Mappers;

public static class LoanMapper
{
    public static LoanResponseDto ToDto(this Loan loan)
    {
        return new LoanResponseDto
        {
            Id = loan.Id,
            MemberId = loan.MemberId,
            BookId = loan.BookId,
            Status = loan.Status,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            CreatedAt = loan.CreatedAt,
            UpdatedAt = loan.UpdatedAt,
            Member = loan.Member?.ToDto()
        };
    }

    public static LoanWithDetailsDto ToWithDetailsDto(this Loan loan)
    {
        var today = DateTime.UtcNow.Date;
        var isOverdue = loan.Status == LoanStatus.Borrowed && loan.DueDate < today;
        
        return new LoanWithDetailsDto
        {
            Id = loan.Id,
            Status = loan.Status,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            IsOverdue = isOverdue,
            BookId = loan.BookId,
            BookTitle = loan.Book?.Title ?? string.Empty,
            BookAuthor = loan.Book?.Author ?? string.Empty,
            BookCoverImageUrl = loan.Book?.CoverImageUrl,
            MemberId = loan.MemberId,
            MemberName = $"{loan.Member?.FirstName} {loan.Member?.LastName}".Trim(),
            MemberEmail = string.Empty
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
        var daysOverdue = (int)(DateTime.UtcNow.Date - loan.DueDate.Date).TotalDays;
        
        return new OverdueLoanDto
        {
            Id = loan.Id,
            DueDate = loan.DueDate,
            DaysOverdue = daysOverdue,
            BookTitle = loan.Book?.Title ?? string.Empty,
            MemberName = $"{loan.Member?.FirstName} {loan.Member?.LastName}".Trim(),
            MemberEmail = string.Empty
        };
    }

    public static Loan ToModel(this CreateLoanDto dto)
    {
        return new Loan
        {
            MemberId = dto.MemberId,
            BookId = dto.BookId,
            DueDate = dto.DueDate ?? DateTime.UtcNow.AddDays(14)
        };
    }

    public static void UpdateModel(this UpdateLoanDto dto, Loan loan)
    {
        if (dto.DueDate.HasValue)
        {
            loan.DueDate = dto.DueDate.Value;
        }
        loan.Status = dto.Status;
    }
} 