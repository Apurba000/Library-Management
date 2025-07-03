using LibraryManagement.Core.Models;

namespace LibraryManagement.Books.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(int id);
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<IEnumerable<Category>> GetCategoriesWithBookCountAsync();
    Task<bool> IsCategoryNameUniqueAsync(string name, int? excludeId = null);
    Task<int> GetBookCountByCategoryAsync(int categoryId);
} 