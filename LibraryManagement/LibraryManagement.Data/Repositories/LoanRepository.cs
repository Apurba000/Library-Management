using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Enums;
using LibraryManagement.Core.Repositories;

namespace LibraryManagement.Data.Repositories;

public class LoanRepository : Repository<Loan>, ILoanRepository
{
    public LoanRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        return await _dbSet
            .Where(l => l.Status == LoanStatus.Borrowed)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansWithDetailsAsync()
    {
        return await _dbSet
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Include(l => l.Member!.User)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByMemberAsync(int memberId)
    {
        return await _dbSet
            .Include(l => l.Book)
            .Where(l => l.MemberId == memberId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByBookAsync(int bookId)
    {
        return await _dbSet
            .Include(l => l.Member)
            .Include(l => l.Member!.User)
            .Where(l => l.BookId == bookId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Include(l => l.Member!.User)
            .Where(l => l.Status == LoanStatus.Borrowed && l.DueDate < today)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByStatusAsync(string status)
    {
        if (Enum.TryParse<LoanStatus>(status, true, out var loanStatus))
        {
            return await _dbSet
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.Status == loanStatus)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        return Enumerable.Empty<Loan>();
    }

    public async Task<int> GetActiveLoansCountByMemberAsync(int memberId)
    {
        return await _dbSet
            .CountAsync(l => l.MemberId == memberId && l.Status == LoanStatus.Borrowed);
    }

    public async Task<int> GetActiveLoansCountByBookAsync(int bookId)
    {
        return await _dbSet
            .CountAsync(l => l.BookId == bookId && l.Status == LoanStatus.Borrowed);
    }

    public async Task<bool> HasActiveLoanAsync(int memberId, int bookId)
    {
        return await _dbSet
            .AnyAsync(l => l.MemberId == memberId && l.BookId == bookId && l.Status == LoanStatus.Borrowed);
    }
} 