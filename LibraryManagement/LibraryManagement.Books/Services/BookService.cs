using LibraryManagement.Core.Models;
using LibraryManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Core.Enums;

namespace LibraryManagement.Books.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly LibraryDbContext _context;

    public BookService(IBookRepository bookRepository, LibraryDbContext context)
    {
        _bookRepository = bookRepository;
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetBooksWithCategoryAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<Book> CreateBookAsync(Book book)
    {
        // Use transaction for creating book with validation
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Business logic: Set default values
            book.CreatedAt = DateTime.UtcNow;
            book.UpdatedAt = DateTime.UtcNow;
            book.IsActive = true;
            
            // Business logic: Validate ISBN uniqueness
            if (!await _bookRepository.IsIsbnUniqueAsync(book.ISBN))
            {
                throw new InvalidOperationException($"Book with ISBN {book.ISBN} already exists.");
            }

            // Add book to repository
            await _bookRepository.AddAsync(book);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            return book;
        }
        catch
        {
            // Rollback transaction on any error
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Book> UpdateBookAsync(Book book)
    {
        // Use transaction for updating book
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingBook = await _bookRepository.GetByIdAsync(book.Id);
            if (existingBook == null)
            {
                throw new InvalidOperationException($"Book with ID {book.Id} not found.");
            }

            // Business logic: Validate ISBN uniqueness (excluding current book)
            if (!await _bookRepository.IsIsbnUniqueAsync(book.ISBN, book.Id))
            {
                throw new InvalidOperationException($"Book with ISBN {book.ISBN} already exists.");
            }

            // Business logic: Update only allowed fields
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Publisher = book.Publisher;
            existingBook.PublicationYear = book.PublicationYear;
            existingBook.Genre = book.Genre;
            existingBook.Description = book.Description;
            existingBook.TotalCopies = book.TotalCopies;
            existingBook.Location = book.Location;
            existingBook.CoverImageUrl = book.CoverImageUrl;
            existingBook.CategoryId = book.CategoryId;
            existingBook.UpdatedAt = DateTime.UtcNow;

            // Update book in repository
            await _bookRepository.UpdateAsync(existingBook);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            return existingBook;
        }
        catch
        {
            // Rollback transaction on any error
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteBookAsync(int id)
    {
        // Use transaction for deleting book
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                throw new InvalidOperationException($"Book with ID {id} not found.");
            }

            // Business logic: Check if book can be deleted (no active loans)
            var activeLoans = await _context.Loans
                .CountAsync(l => l.BookId == id && l.Status == LoanStatus.Borrowed);
            
            if (activeLoans > 0)
            {
                throw new InvalidOperationException($"Cannot delete book with ID {id}. It has {activeLoans} active loan(s).");
            }

            // Business logic: Soft delete instead of hard delete
            book.IsActive = false;
            book.UpdatedAt = DateTime.UtcNow;
            
            // Update book in repository
            await _bookRepository.UpdateAsync(book);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
        }
        catch
        {
            // Rollback transaction on any error
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
    {
        return await _bookRepository.GetBooksByAuthorAsync(author);
    }

    public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
    {
        return await _bookRepository.GetBooksByCategoryAsync(categoryId);
    }

    public async Task<bool> IsBookAvailableAsync(int bookId)
    {
        var availableCopies = await _bookRepository.GetAvailableCopiesCountAsync(bookId);
        return availableCopies > 0;
    }

    public async Task<int> GetAvailableCopiesAsync(int bookId)
    {
        return await _bookRepository.GetAvailableCopiesCountAsync(bookId);
    }

    public async Task<Book> DecrementAvailableCopiesAsync(int bookId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
            {
                throw new InvalidOperationException($"Book with ID {bookId} not found.");
            }

            if (!book.IsActive)
            {
                throw new InvalidOperationException($"Book with ID {bookId} is not active.");
            }

            if (book.AvailableCopies <= 0)
            {
                throw new InvalidOperationException($"Book with ID {bookId} has no available copies to decrement.");
            }

            book.AvailableCopies--;
            book.UpdatedAt = DateTime.UtcNow;

            await _bookRepository.UpdateAsync(book);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return book;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
} 