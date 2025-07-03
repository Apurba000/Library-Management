using LibraryManagement.Core.Models;

namespace LibraryManagement.Core.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    // Category-specific operations
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<IEnumerable<Category>> GetCategoriesWithBookCountAsync();
    Task<bool> IsCategoryNameUniqueAsync(string name, int? excludeId = null);
    Task<int> GetBookCountByCategoryAsync(int categoryId);
} 