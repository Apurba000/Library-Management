using LibraryManagement.Models;
using LibraryManagement.DTOs;
using LibraryManagement.Enums;

namespace LibraryManagement.Mappers;

public static class MemberMapper
{
    public static MemberResponseDto ToDto(this Member member)
    {
        return new MemberResponseDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Phone = member.Phone,
            Address = member.Address,
            DateOfBirth = member.DateOfBirth,
            MembershipStatus = member.MembershipStatus,
            IsActive = member.IsActive,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt,
            ActiveLoansCount = member.Loans?.Count(l => l.Status == LoanStatus.Borrowed) ?? 0
        };
    }

    public static MemberWithLoansDto ToWithLoansDto(this Member member)
    {
        return new MemberWithLoansDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            MembershipStatus = member.MembershipStatus,
            ActiveLoansCount = member.Loans?.Count(l => l.Status == LoanStatus.Borrowed) ?? 0,
            ActiveLoans = member.Loans?
                .Where(l => l.Status == LoanStatus.Borrowed)
                .Select(l => l.ToSummaryDto())
                .ToList() ?? new List<LoanSummaryDto>()
        };
    }

    public static MemberSummaryDto ToSummaryDto(this Member member, int activeLoansCount = 0)
    {
        return new MemberSummaryDto
        {
            Id = member.Id,
            MemberNumber = member.MemberNumber,
            FullName = $"{member.FirstName} {member.LastName}",
            Status = member.MembershipStatus,
            ActiveLoansCount = activeLoansCount
        };
    }

    public static Member ToModel(this CreateMemberDto dto)
    {
        return new Member
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Address = dto.Address,
            DateOfBirth = dto.DateOfBirth,
            UserId = dto.UserId
        };
    }

    public static void UpdateModel(this UpdateMemberDto dto, Member member)
    {
        member.FirstName = dto.FirstName;
        member.LastName = dto.LastName;
        member.Phone = dto.Phone;
        member.Address = dto.Address;
        member.DateOfBirth = dto.DateOfBirth;
        member.MembershipStatus = dto.MembershipStatus;
    }
} 