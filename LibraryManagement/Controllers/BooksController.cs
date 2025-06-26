using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.DTOs;
using LibraryManagement.Mappers;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET: api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetBooks()
    {
        try
        {
            var books = await _bookService.GetAllBooksAsync();
            var bookDtos = books.Select(b => b.ToDto());
            return Ok(bookDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving books", details = ex.Message });
        }
    }

    // GET: api/books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponseDto>> GetBook(int id)
    {
        try
        {
            var book = await _bookService.GetBookByIdAsync(id);
            
            if (book == null)
            {
                return NotFound(new { error = $"Book with ID {id} not found" });
            }

            return Ok(book.ToDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the book", details = ex.Message });
        }
    }

    // GET: api/books/author/{author}
    [HttpGet("author/{author}")]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetBooksByAuthor(string author)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                return BadRequest(new { error = "Author name cannot be empty" });
            }

            var books = await _bookService.GetBooksByAuthorAsync(author);
            var bookDtos = books.Select(b => b.ToDto());
            return Ok(bookDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving books by author", details = ex.Message });
        }
    }

    // GET: api/books/category/{categoryId}
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetBooksByCategory(int categoryId)
    {
        try
        {
            if (categoryId <= 0)
            {
                return BadRequest(new { error = "Category ID must be greater than 0" });
            }

            var books = await _bookService.GetBooksByCategoryAsync(categoryId);
            var bookDtos = books.Select(b => b.ToDto());
            return Ok(bookDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving books by category", details = ex.Message });
        }
    }

    // GET: api/books/{id}/availability
    [HttpGet("{id}/availability")]
    public async Task<ActionResult<BookAvailabilityDto>> GetBookAvailability(int id)
    {
        try
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound(new { error = $"Book with ID {id} not found" });
            }

            var isAvailable = await _bookService.IsBookAvailableAsync(id);
            var availableCopies = await _bookService.GetAvailableCopiesAsync(id);

            return Ok(book.ToAvailabilityDto(isAvailable, availableCopies));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while checking book availability", details = ex.Message });
        }
    }

    // POST: api/books
    [HttpPost]
    public async Task<ActionResult<BookResponseDto>> CreateBook([FromBody] CreateBookDto createBookDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = createBookDto.ToModel();
            var createdBook = await _bookService.CreateBookAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the book", details = ex.Message });
        }
    }

    // PUT: api/books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBook = await _bookService.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                return NotFound(new { error = $"Book with ID {id} not found" });
            }

            updateBookDto.UpdateModel(existingBook);
            var updatedBook = await _bookService.UpdateBookAsync(existingBook);
            return Ok(updatedBook.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating the book", details = ex.Message });
        }
    }

    // DELETE: api/books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the book", details = ex.Message });
        }
    }
} 