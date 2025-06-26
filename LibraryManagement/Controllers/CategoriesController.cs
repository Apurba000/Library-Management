using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.DTOs;
using LibraryManagement.Mappers;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
    {
        try
        {
            var categories = await _categoryService.GetActiveCategoriesAsync();
            var categoryDtos = categories.Select(c => c.ToDto());
            return Ok(categoryDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving categories", details = ex.Message });
        }
    }

    // GET: api/categories/with-book-count
    [HttpGet("with-book-count")]
    public async Task<ActionResult<IEnumerable<CategoryWithBookCountDto>>> GetCategoriesWithBookCount()
    {
        try
        {
            var categories = await _categoryService.GetCategoriesWithBookCountAsync();
            var categoryDtos = categories.Select(c => c.ToBookCountDto());
            return Ok(categoryDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving categories with book count", details = ex.Message });
        }
    }

    // GET: api/categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            
            if (category == null)
            {
                return NotFound(new { error = $"Category with ID {id} not found" });
            }

            return Ok(category.ToDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the category", details = ex.Message });
        }
    }

    // GET: api/categories/5/book-count
    [HttpGet("{id}/book-count")]
    public async Task<ActionResult<int>> GetCategoryBookCount(int id)
    {
        try
        {
            var bookCount = await _categoryService.GetBookCountByCategoryAsync(id);
            return Ok(new { categoryId = id, bookCount });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the book count", details = ex.Message });
        }
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = createCategoryDto.ToModel();
            var createdCategory = await _categoryService.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the category", details = ex.Message });
        }
    }

    // PUT: api/categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { error = $"Category with ID {id} not found" });
            }

            updateCategoryDto.UpdateModel(existingCategory);
            var updatedCategory = await _categoryService.UpdateCategoryAsync(existingCategory);
            return Ok(updatedCategory.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating the category", details = ex.Message });
        }
    }

    // DELETE: api/categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the category", details = ex.Message });
        }
    }

    // GET: api/categories/check-name-unique?name=Fantasy&excludeId=5
    [HttpGet("check-name-unique")]
    public async Task<ActionResult<bool>> IsCategoryNameUnique([FromQuery] string name, [FromQuery] int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "Category name cannot be empty" });
            }

            var isUnique = await _categoryService.IsCategoryNameUniqueAsync(name, excludeId);
            return Ok(new { name, excludeId, isUnique });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while checking category name uniqueness", details = ex.Message });
        }
    }
} 