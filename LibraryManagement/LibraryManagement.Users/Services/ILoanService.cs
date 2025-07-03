using LibraryManagement.Core.Models;

namespace LibraryManagement.Users.Services;

public interface ILoanService
{
    Task<IEnumerable<Loan>> GetAllLoansAsync();
    Task<Loan?> GetLoanByIdAsync(int id);
    Task<Loan> CreateLoanAsync(Loan loan);
    Task<Loan> UpdateLoanAsync(Loan loan);
    Task DeleteLoanAsync(int id);
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetLoansWithDetailsAsync();
    Task<IEnumerable<Loan>> GetLoansByMemberAsync(int memberId);
    Task<IEnumerable<Loan>> GetLoansByBookAsync(int bookId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetLoansByStatusAsync(string status);
    Task<int> GetActiveLoansCountByMemberAsync(int memberId);
    Task<int> GetActiveLoansCountByBookAsync(int bookId);
    Task<bool> HasActiveLoanAsync(int memberId, int bookId);
    Task<Loan> BorrowBookAsync(int memberId, int bookId);
    Task<Loan> ReturnBookAsync(int loanId);
} 