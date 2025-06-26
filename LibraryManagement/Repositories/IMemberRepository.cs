using LibraryManagement.Models;

namespace LibraryManagement.Repositories;

public interface IMemberRepository : IRepository<Member>
{
    // Member-specific operations
    Task<IEnumerable<Member>> GetActiveMembersAsync();
    Task<IEnumerable<Member>> GetMembersWithUserInfoAsync();
    Task<Member?> GetMemberByUserIdAsync(int userId);
    Task<bool> IsPhoneUniqueAsync(string phone, int? excludeId = null);
    Task<int> GetActiveLoansCountAsync(int memberId);
    Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync();
} 