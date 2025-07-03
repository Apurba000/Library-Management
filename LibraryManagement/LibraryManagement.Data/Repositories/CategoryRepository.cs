using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Repositories;

namespace LibraryManagement.Data.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithBookCountAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .Include(c => c.Books.Where(b => b.IsActive))
            .Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                Books = c.Books
            })
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> IsCategoryNameUniqueAsync(string name, int? excludeId = null)
    {
        var query = _dbSet.Where(c => c.Name.ToLower() == name.ToLower() && c.IsActive);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<int> GetBookCountByCategoryAsync(int categoryId)
    {
        return await _context.Books
            .CountAsync(b => b.CategoryId == categoryId && b.IsActive);
    }
} 