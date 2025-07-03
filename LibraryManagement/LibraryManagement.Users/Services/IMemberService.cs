using LibraryManagement.Core.Models;

namespace LibraryManagement.Users.Services;

public interface IMemberService
{
    Task<IEnumerable<Member>> GetAllMembersAsync();
    Task<Member?> GetMemberByIdAsync(int id);
    Task<Member> CreateMemberAsync(Member member);
    Task<Member> UpdateMemberAsync(Member member);
    Task DeleteMemberAsync(int id);
    Task<IEnumerable<Member>> GetActiveMembersAsync();
    Task<IEnumerable<Member>> GetMembersWithUserInfoAsync();
    Task<Member?> GetMemberByUserIdAsync(int userId);
    Task<bool> IsPhoneUniqueAsync(string phone, int? excludeId = null);
    Task<int> GetActiveLoansCountAsync(int memberId);
    Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync();
} 