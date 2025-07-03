using LibraryManagement.Core.Models;

namespace LibraryManagement.Books.Services;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> CreateBookAsync(Book book);
    Task<Book> UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);
    Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);
    Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
    Task<bool> IsBookAvailableAsync(int bookId);
    Task<int> GetAvailableCopiesAsync(int bookId);
    Task<Book> DecrementAvailableCopiesAsync(int bookId);
}