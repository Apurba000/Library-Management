using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.DTOs;
using LibraryManagement.Mappers;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    // GET: api/loans
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanResponseDto>>> GetLoans()
    {
        try
        {
            var loans = await _loanService.GetLoansWithDetailsAsync();
            var loanDtos = loans.Select(l => l.ToDto());
            return Ok(loanDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving loans", details = ex.Message });
        }
    }

    // GET: api/loans/active
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<LoanResponseDto>>> GetActiveLoans()
    {
        try
        {
            var loans = await _loanService.GetActiveLoansAsync();
            var loanDtos = loans.Select(l => l.ToDto());
            return Ok(loanDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving active loans", details = ex.Message });
        }
    }

    // GET: api/loans/overdue
    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<OverdueLoanDto>>> GetOverdueLoans()
    {
        try
        {
            var loans = await _loanService.GetOverdueLoansAsync();
            var loanDtos = loans.Select(l => l.ToOverdueDto());
            return Ok(loanDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving overdue loans", details = ex.Message });
        }
    }

    // GET: api/loans/by-status?status=Borrowed
    [HttpGet("by-status")]
    public async Task<ActionResult<IEnumerable<LoanResponseDto>>> GetLoansByStatus([FromQuery] string status)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new { error = "Status cannot be empty" });
            }

            var loans = await _loanService.GetLoansByStatusAsync(status);
            var loanDtos = loans.Select(l => l.ToDto());
            return Ok(loanDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving loans by status", details = ex.Message });
        }
    }

    // GET: api/loans/by-member/5
    [HttpGet("by-member/{memberId}")]
    public async Task<ActionResult<IEnumerable<LoanResponseDto>>> GetLoansByMember(int memberId)
    {
        try
        {
            var loans = await _loanService.GetLoansByMemberAsync(memberId);
            var loanDtos = loans.Select(l => l.ToDto());
            return Ok(loanDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving loans by member", details = ex.Message });
        }
    }

    // GET: api/loans/by-book/5
    [HttpGet("by-book/{bookId}")]
    public async Task<ActionResult<IEnumerable<LoanResponseDto>>> GetLoansByBook(int bookId)
    {
        try
        {
            var loans = await _loanService.GetLoansByBookAsync(bookId);
            var loanDtos = loans.Select(l => l.ToDto());
            return Ok(loanDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving loans by book", details = ex.Message });
        }
    }

    // GET: api/loans/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LoanResponseDto>> GetLoan(int id)
    {
        try
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            
            if (loan == null)
            {
                return NotFound(new { error = $"Loan with ID {id} not found" });
            }

            return Ok(loan.ToDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the loan", details = ex.Message });
        }
    }

    // GET: api/loans/5/with-details
    [HttpGet("{id}/with-details")]
    public async Task<ActionResult<LoanWithDetailsDto>> GetLoanWithDetails(int id)
    {
        try
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            
            if (loan == null)
            {
                return NotFound(new { error = $"Loan with ID {id} not found" });
            }

            return Ok(loan.ToWithDetailsDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the loan details", details = ex.Message });
        }
    }

    // GET: api/loans/member/5/active-count
    [HttpGet("member/{memberId}/active-count")]
    public async Task<ActionResult<int>> GetMemberActiveLoansCount(int memberId)
    {
        try
        {
            var activeLoansCount = await _loanService.GetActiveLoansCountByMemberAsync(memberId);
            return Ok(new { memberId, activeLoansCount });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the active loans count", details = ex.Message });
        }
    }

    // GET: api/loans/book/5/active-count
    [HttpGet("book/{bookId}/active-count")]
    public async Task<ActionResult<int>> GetBookActiveLoansCount(int bookId)
    {
        try
        {
            var activeLoansCount = await _loanService.GetActiveLoansCountByBookAsync(bookId);
            return Ok(new { bookId, activeLoansCount });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the active loans count", details = ex.Message });
        }
    }

    // GET: api/loans/check-active?memberId=5&bookId=10
    [HttpGet("check-active")]
    public async Task<ActionResult<bool>> HasActiveLoan([FromQuery] int memberId, [FromQuery] int bookId)
    {
        try
        {
            var hasActiveLoan = await _loanService.HasActiveLoanAsync(memberId, bookId);
            return Ok(new { memberId, bookId, hasActiveLoan });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while checking active loan", details = ex.Message });
        }
    }

    // POST: api/loans
    [HttpPost]
    public async Task<ActionResult<LoanResponseDto>> CreateLoan([FromBody] CreateLoanDto createLoanDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loan = createLoanDto.ToModel();
            var createdLoan = await _loanService.CreateLoanAsync(loan);
            return CreatedAtAction(nameof(GetLoan), new { id = createdLoan.Id }, createdLoan.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the loan", details = ex.Message });
        }
    }

    // POST: api/loans/borrow
    [HttpPost("borrow")]
    public async Task<ActionResult<LoanResponseDto>> BorrowBook([FromBody] BorrowBookDto borrowBookDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loan = await _loanService.BorrowBookAsync(borrowBookDto.MemberId, borrowBookDto.BookId);
            return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while borrowing the book", details = ex.Message });
        }
    }

    // POST: api/loans/return
    [HttpPost("return")]
    public async Task<ActionResult<LoanResponseDto>> ReturnBook([FromBody] ReturnBookDto returnBookDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loan = await _loanService.ReturnBookAsync(returnBookDto.LoanId);
            return Ok(loan.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while returning the book", details = ex.Message });
        }
    }

    // PUT: api/loans/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoan(int id, [FromBody] UpdateLoanDto updateLoanDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingLoan = await _loanService.GetLoanByIdAsync(id);
            if (existingLoan == null)
            {
                return NotFound(new { error = $"Loan with ID {id} not found" });
            }

            updateLoanDto.UpdateModel(existingLoan);
            var updatedLoan = await _loanService.UpdateLoanAsync(existingLoan);
            return Ok(updatedLoan.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating the loan", details = ex.Message });
        }
    }

    // DELETE: api/loans/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoan(int id)
    {
        try
        {
            await _loanService.DeleteLoanAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the loan", details = ex.Message });
        }
    }
} 