using LibraryManagement.Core.Models;

namespace LibraryManagement.Core.Repositories;

public interface IBookRepository : IRepository<Book>
{
    // Book-specific operations
    Task<IEnumerable<Book>> GetBooksWithCategoryAsync();
    Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);
    Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<bool> IsIsbnUniqueAsync(string isbn, int? excludeId = null);
    Task<int> GetAvailableCopiesCountAsync(int bookId);
} 