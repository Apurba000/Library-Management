using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.DTOs;
using LibraryManagement.Mappers;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    // GET: api/members
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetMembers()
    {
        try
        {
            var members = await _memberService.GetActiveMembersAsync();
            var memberDtos = members.Select(m => m.ToDto());
            return Ok(memberDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving members", details = ex.Message });
        }
    }

    // GET: api/members/with-user-info
    [HttpGet("with-user-info")]
    public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetMembersWithUserInfo()
    {
        try
        {
            var members = await _memberService.GetMembersWithUserInfoAsync();
            var memberDtos = members.Select(m => m.ToDto());
            return Ok(memberDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving members with user info", details = ex.Message });
        }
    }

    // GET: api/members/with-active-loans
    [HttpGet("with-active-loans")]
    public async Task<ActionResult<IEnumerable<MemberWithLoansDto>>> GetMembersWithActiveLoans()
    {
        try
        {
            var members = await _memberService.GetMembersWithActiveLoansAsync();
            var memberDtos = members.Select(m => m.ToWithLoansDto());
            return Ok(memberDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving members with active loans", details = ex.Message });
        }
    }

    // GET: api/members/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MemberResponseDto>> GetMember(int id)
    {
        try
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            
            if (member == null)
            {
                return NotFound(new { error = $"Member with ID {id} not found" });
            }

            return Ok(member.ToDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the member", details = ex.Message });
        }
    }

    // GET: api/members/by-email?email=john@example.com
    [HttpGet("by-email")]
    public async Task<ActionResult<MemberResponseDto>> GetMemberByEmail([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { error = "Email cannot be empty" });
            }

            // Note: Member model doesn't have Email property, so this endpoint is not available
            return BadRequest(new { error = "Email lookup not available for members" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the member", details = ex.Message });
        }
    }

    // GET: api/members/by-user/5
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<MemberResponseDto>> GetMemberByUserId(int userId)
    {
        try
        {
            var member = await _memberService.GetMemberByUserIdAsync(userId);
            
            if (member == null)
            {
                return NotFound(new { error = $"Member with user ID {userId} not found" });
            }

            return Ok(member.ToDto());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the member", details = ex.Message });
        }
    }

    // GET: api/members/5/active-loans-count
    [HttpGet("{id}/active-loans-count")]
    public async Task<ActionResult<int>> GetMemberActiveLoansCount(int id)
    {
        try
        {
            var activeLoansCount = await _memberService.GetActiveLoansCountAsync(id);
            return Ok(new { memberId = id, activeLoansCount });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the active loans count", details = ex.Message });
        }
    }

    // POST: api/members
    [HttpPost]
    public async Task<ActionResult<MemberResponseDto>> CreateMember([FromBody] CreateMemberDto createMemberDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var member = createMemberDto.ToModel();
            var createdMember = await _memberService.CreateMemberAsync(member);
            return CreatedAtAction(nameof(GetMember), new { id = createdMember.Id }, createdMember.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the member", details = ex.Message });
        }
    }

    // PUT: api/members/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMember(int id, [FromBody] UpdateMemberDto updateMemberDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingMember = await _memberService.GetMemberByIdAsync(id);
            if (existingMember == null)
            {
                return NotFound(new { error = $"Member with ID {id} not found" });
            }

            updateMemberDto.UpdateModel(existingMember);
            var updatedMember = await _memberService.UpdateMemberAsync(existingMember);
            return Ok(updatedMember.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating the member", details = ex.Message });
        }
    }

    // DELETE: api/members/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        try
        {
            await _memberService.DeleteMemberAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the member", details = ex.Message });
        }
    }

    // GET: api/members/check-email-unique?email=john@example.com&excludeId=5
    [HttpGet("check-email-unique")]
    public async Task<ActionResult<bool>> IsEmailUnique([FromQuery] string email, [FromQuery] int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { error = "Email cannot be empty" });
            }

            // Note: Member model doesn't have Email property, so this endpoint is not available
            return BadRequest(new { error = "Email uniqueness check not available for members" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while checking email uniqueness", details = ex.Message });
        }
    }

    // GET: api/members/check-phone-unique?phone=1234567890&excludeId=5
    [HttpGet("check-phone-unique")]
    public async Task<ActionResult<bool>> IsPhoneUnique([FromQuery] string phone, [FromQuery] int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return BadRequest(new { error = "Phone number cannot be empty" });
            }

            var isUnique = await _memberService.IsPhoneUniqueAsync(phone, excludeId);
            return Ok(new { phone, excludeId, isUnique });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while checking phone number uniqueness", details = ex.Message });
        }
    }
} 