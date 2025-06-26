using LibraryManagement.Models;
using LibraryManagement.DTOs;

namespace LibraryManagement.Mappers;

public static class BookMapper
{
    // Convert Book model to BookResponseDto
    public static BookResponseDto ToDto(this Book book)
    {
        return new BookResponseDto
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            PublicationYear = book.PublicationYear,
            Genre = book.Genre,
            Description = book.Description,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            Location = book.Location,
            CoverImageUrl = book.CoverImageUrl,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt
        };
    }

    // Convert CreateBookDto to Book model
    public static Book ToModel(this CreateBookDto dto)
    {
        return new Book
        {
            ISBN = dto.ISBN,
            Title = dto.Title,
            Author = dto.Author,
            Publisher = dto.Publisher,
            PublicationYear = dto.PublicationYear,
            Genre = dto.Genre,
            Description = dto.Description,
            TotalCopies = dto.TotalCopies,
            AvailableCopies = dto.TotalCopies, // Initially all copies are available
            Location = dto.Location,
            CoverImageUrl = dto.CoverImageUrl,
            CategoryId = dto.CategoryId
        };
    }

    // Convert UpdateBookDto to Book model (for updates)
    public static void UpdateModel(this UpdateBookDto dto, Book book)
    {
        book.ISBN = dto.ISBN;
        book.Title = dto.Title;
        book.Author = dto.Author;
        book.Publisher = dto.Publisher;
        book.PublicationYear = dto.PublicationYear;
        book.Genre = dto.Genre;
        book.Description = dto.Description;
        book.TotalCopies = dto.TotalCopies;
        book.Location = dto.Location;
        book.CoverImageUrl = dto.CoverImageUrl;
        book.CategoryId = dto.CategoryId;
        book.UpdatedAt = DateTime.UtcNow;
    }

    // Convert Book to BookAvailabilityDto
    public static BookAvailabilityDto ToAvailabilityDto(this Book book, bool isAvailable, int availableCopies)
    {
        return new BookAvailabilityDto
        {
            BookId = book.Id,
            Title = book.Title,
            IsAvailable = isAvailable,
            AvailableCopies = availableCopies,
            TotalCopies = book.TotalCopies
        };
    }
} 