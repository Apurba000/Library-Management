using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Enums;
using LibraryManagement.Core.Repositories;

namespace LibraryManagement.Data.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetBooksWithCategoryAsync()
    {
        return await _dbSet
            .Include(b => b.Category)
            .Include(b => b.CreatedByUser)
            .Where(b => b.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
    {
        return await _dbSet
            .Include(b => b.Category)
            .Where(b => b.Author.Contains(author) && b.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Include(b => b.Category)
            .Where(b => b.CategoryId == categoryId && b.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _dbSet
            .Include(b => b.Category)
            .Where(b => b.IsActive && b.AvailableCopies > 0)
            .ToListAsync();
    }

    public async Task<bool> IsIsbnUniqueAsync(string isbn, int? excludeId = null)
    {
        var query = _dbSet.Where(b => b.ISBN == isbn && b.IsActive);
        
        if (excludeId.HasValue)
        {
            query = query.Where(b => b.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<int> GetAvailableCopiesCountAsync(int bookId)
    {
        var book = await _dbSet.FindAsync(bookId);
        if (book == null || !book.IsActive)
        {
            return 0;
        }

        // Count active loans for this book
        var activeLoans = await _context.Loans
            .CountAsync(l => l.BookId == bookId && l.Status == LoanStatus.Borrowed);

        return Math.Max(0, book.TotalCopies - activeLoans);
    }
} 