using LibraryManagement.Models;
using LibraryManagement.DTOs;

namespace LibraryManagement.Mappers;

public static class MemberMapper
{
    public static MemberResponseDto ToDto(this Member member)
    {
        return new MemberResponseDto
        {
            Id = member.Id,
            MemberNumber = member.MemberNumber,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Phone = member.Phone ?? string.Empty,
            Email = member.User?.Email ?? string.Empty,
            Address = member.Address ?? string.Empty,
            DateOfBirth = member.DateOfBirth ?? DateTime.MinValue,
            Status = member.MembershipStatus,
            MembershipStartDate = member.MembershipDate,
            MembershipEndDate = member.MembershipExpiryDate,
            Notes = string.Empty,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt,
            User = member.User?.ToDto()
        };
    }

    public static MemberSummaryDto ToSummaryDto(this Member member, int activeLoansCount = 0)
    {
        return new MemberSummaryDto
        {
            Id = member.Id,
            MemberNumber = member.MemberNumber,
            FullName = $"{member.FirstName} {member.LastName}",
            Email = member.User?.Email ?? string.Empty,
            Status = member.MembershipStatus,
            ActiveLoansCount = activeLoansCount
        };
    }

    public static Member ToModel(this CreateMemberDto dto)
    {
        return new Member
        {
            MemberNumber = dto.MemberNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Address = dto.Address,
            DateOfBirth = dto.DateOfBirth,
            MembershipStatus = dto.Status,
            MembershipDate = dto.MembershipStartDate,
            MembershipExpiryDate = dto.MembershipEndDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateModel(this UpdateMemberDto dto, Member member)
    {
        member.MemberNumber = dto.MemberNumber;
        member.FirstName = dto.FirstName;
        member.LastName = dto.LastName;
        member.Phone = dto.Phone;
        member.Address = dto.Address;
        member.DateOfBirth = dto.DateOfBirth;
        member.MembershipStatus = dto.Status;
        member.MembershipDate = dto.MembershipStartDate;
        member.MembershipExpiryDate = dto.MembershipEndDate;
        member.UpdatedAt = DateTime.UtcNow;
    }
} 