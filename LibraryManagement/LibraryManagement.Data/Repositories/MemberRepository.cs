using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Enums;
using LibraryManagement.Core.Repositories;

namespace LibraryManagement.Data.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        return await _dbSet
            .Where(m => m.IsActive)
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersWithUserInfoAsync()
    {
        return await _dbSet
            .Include(m => m.User)
            .Where(m => m.IsActive)
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync();
    }

    public async Task<Member?> GetMemberByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone, int? excludeId = null)
    {
        var query = _dbSet.Where(m => m.Phone == phone && m.IsActive);
        
        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<int> GetActiveLoansCountAsync(int memberId)
    {
        return await _context.Loans
            .CountAsync(l => l.MemberId == memberId && l.Status == LoanStatus.Borrowed);
    }

    public async Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync()
    {
        return await _dbSet
            .Include(m => m.Loans.Where(l => l.Status == LoanStatus.Borrowed))
            .Where(m => m.IsActive && m.Loans.Any(l => l.Status == LoanStatus.Borrowed))
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync();
    }
} 