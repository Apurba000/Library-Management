using LibraryManagement.Core.Models;
using LibraryManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;

namespace LibraryManagement.Books.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly LibraryDbContext _context;

    public CategoryService(ICategoryRepository categoryRepository, LibraryDbContext context)
    {
        _categoryRepository = categoryRepository;
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Business logic: Set default values
            category.CreatedAt = DateTime.UtcNow;
            category.IsActive = true;
            
            // Business logic: Validate name uniqueness
            if (!await _categoryRepository.IsCategoryNameUniqueAsync(category.Name))
            {
                throw new InvalidOperationException($"Category with name '{category.Name}' already exists.");
            }

            // Add category to repository
            await _categoryRepository.AddAsync(category);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            return category;
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            await transaction.RollbackAsync();
            
            switch (pgEx.SqlState)
            {
                case "23505": // Unique constraint violation
                    throw new InvalidOperationException($"Category with name '{category.Name}' already exists.");
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

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(category.Id);
            if (existingCategory == null)
            {
                throw new InvalidOperationException($"Category with ID {category.Id} not found.");
            }

            // Business logic: Validate name uniqueness (excluding current category)
            if (!await _categoryRepository.IsCategoryNameUniqueAsync(category.Name, category.Id))
            {
                throw new InvalidOperationException($"Category with name '{category.Name}' already exists.");
            }

            // Business logic: Update only allowed fields
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            // Update category in repository
            await _categoryRepository.UpdateAsync(existingCategory);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            return existingCategory;
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            await transaction.RollbackAsync();
            
            switch (pgEx.SqlState)
            {
                case "23505": // Unique constraint violation
                    throw new InvalidOperationException($"Category with name '{category.Name}' already exists.");
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

    public async Task DeleteCategoryAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {id} not found.");
            }

            // Business logic: Check if category can be deleted (no active books)
            var bookCount = await _categoryRepository.GetBookCountByCategoryAsync(id);
            
            if (bookCount > 0)
            {
                throw new InvalidOperationException($"Cannot delete category with ID {id}. It has {bookCount} active book(s).");
            }

            // Business logic: Soft delete instead of hard delete
            category.IsActive = false;
            
            // Update category in repository
            await _categoryRepository.UpdateAsync(category);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _categoryRepository.GetActiveCategoriesAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithBookCountAsync()
    {
        return await _categoryRepository.GetCategoriesWithBookCountAsync();
    }

    public async Task<bool> IsCategoryNameUniqueAsync(string name, int? excludeId = null)
    {
        return await _categoryRepository.IsCategoryNameUniqueAsync(name, excludeId);
    }

    public async Task<int> GetBookCountByCategoryAsync(int categoryId)
    {
        return await _categoryRepository.GetBookCountByCategoryAsync(categoryId);
    }
} 