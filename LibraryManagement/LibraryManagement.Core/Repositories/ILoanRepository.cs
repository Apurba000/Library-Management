using LibraryManagement.Core.Models;

namespace LibraryManagement.Core.Repositories;

public interface ILoanRepository : IRepository<Loan>
{
    // Loan-specific operations
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetLoansWithDetailsAsync();
    Task<IEnumerable<Loan>> GetLoansByMemberAsync(int memberId);
    Task<IEnumerable<Loan>> GetLoansByBookAsync(int bookId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetLoansByStatusAsync(string status);
    Task<int> GetActiveLoansCountByMemberAsync(int memberId);
    Task<int> GetActiveLoansCountByBookAsync(int bookId);
    Task<bool> HasActiveLoanAsync(int memberId, int bookId);
} 