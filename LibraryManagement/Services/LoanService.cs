using LibraryManagement.Models;
using LibraryManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Enums;

namespace LibraryManagement.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly LibraryDbContext _context;

    public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, IMemberRepository memberRepository, LibraryDbContext context)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _context = context;
    }

    public async Task<IEnumerable<Loan>> GetAllLoansAsync()
    {
        return await _loanRepository.GetAllAsync();
    }

    public async Task<Loan?> GetLoanByIdAsync(int id)
    {
        return await _loanRepository.GetByIdAsync(id);
    }

    public async Task<Loan> CreateLoanAsync(Loan loan)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            loan.CreatedAt = DateTime.UtcNow;
            loan.UpdatedAt = DateTime.UtcNow;
            loan.Status = LoanStatus.Borrowed;
            loan.LoanDate = DateTime.UtcNow;
            loan.DueDate = DateTime.UtcNow.AddDays(14);
            
            await _loanRepository.AddAsync(loan);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return loan;
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            await transaction.RollbackAsync();
            
            switch (pgEx.SqlState)
            {
                case "23503":
                    if (pgEx.ConstraintName?.Contains("BookId") == true)
                    {
                        throw new InvalidOperationException($"Book with ID {loan.BookId} does not exist.");
                    }
                    else if (pgEx.ConstraintName?.Contains("MemberId") == true)
                    {
                        throw new InvalidOperationException($"Member with ID {loan.MemberId} does not exist.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Referenced record does not exist: {pgEx.ConstraintName}");
                    }
                
                default:
                    throw new InvalidOperationException($"Database error: {pgEx.MessageText}");
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Loan> UpdateLoanAsync(Loan loan)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingLoan = await _loanRepository.GetByIdAsync(loan.Id);
            if (existingLoan == null)
            {
                throw new InvalidOperationException($"Loan with ID {loan.Id} not found.");
            }

            existingLoan.DueDate = loan.DueDate;
            existingLoan.Status = loan.Status;
            existingLoan.UpdatedAt = DateTime.UtcNow;

            if (loan.Status == LoanStatus.Returned && existingLoan.ReturnDate == null)
            {
                existingLoan.ReturnDate = DateTime.UtcNow;
            }

            await _loanRepository.UpdateAsync(existingLoan);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return existingLoan;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteLoanAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                throw new InvalidOperationException($"Loan with ID {id} not found.");
            }

            if (loan.Status == LoanStatus.Borrowed)
            {
                throw new InvalidOperationException($"Cannot delete loan with ID {id}. Book is still borrowed.");
            }

            await _loanRepository.DeleteAsync(loan);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        return await _loanRepository.GetActiveLoansAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansWithDetailsAsync()
    {
        return await _loanRepository.GetLoansWithDetailsAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByMemberAsync(int memberId)
    {
        return await _loanRepository.GetLoansByMemberAsync(memberId);
    }

    public async Task<IEnumerable<Loan>> GetLoansByBookAsync(int bookId)
    {
        return await _loanRepository.GetLoansByBookAsync(bookId);
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        return await _loanRepository.GetOverdueLoansAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByStatusAsync(string status)
    {
        return await _loanRepository.GetLoansByStatusAsync(status);
    }

    public async Task<int> GetActiveLoansCountByMemberAsync(int memberId)
    {
        return await _loanRepository.GetActiveLoansCountByMemberAsync(memberId);
    }

    public async Task<int> GetActiveLoansCountByBookAsync(int bookId)
    {
        return await _loanRepository.GetActiveLoansCountByBookAsync(bookId);
    }

    public async Task<bool> HasActiveLoanAsync(int memberId, int bookId)
    {
        return await _loanRepository.HasActiveLoanAsync(memberId, bookId);
    }

    public async Task<Loan> BorrowBookAsync(int memberId, int bookId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null || !member.IsActive)
            {
                throw new InvalidOperationException($"Member with ID {memberId} not found or inactive.");
            }

            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null || !book.IsActive)
            {
                throw new InvalidOperationException($"Book with ID {bookId} not found or inactive.");
            }

            // Check if member already has this book borrowed
            if (await HasActiveLoanAsync(memberId, bookId))
            {
                throw new InvalidOperationException($"Member {memberId} already has book {bookId} borrowed.");
            }

            var loan = new Loan
            {
                MemberId = memberId,
                BookId = bookId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = LoanStatus.Borrowed,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _loanRepository.AddAsync(loan);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return loan;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Loan> ReturnBookAsync(int loanId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new InvalidOperationException($"Loan with ID {loanId} not found.");
            }

            if (loan.Status != LoanStatus.Borrowed)
            {
                throw new InvalidOperationException($"Loan with ID {loanId} is not in borrowed status.");
            }

            loan.Status = LoanStatus.Returned;
            loan.ReturnDate = DateTime.UtcNow;
            loan.UpdatedAt = DateTime.UtcNow;

            await _loanRepository.UpdateAsync(loan);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return loan;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
} 