using LibraryManagement.Core.Models;
using LibraryManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Core.Enums;

namespace LibraryManagement.Users.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly LibraryDbContext _context;

    public MemberService(IMemberRepository memberRepository, LibraryDbContext context)
    {
        _memberRepository = memberRepository;
        _context = context;
    }

    public async Task<IEnumerable<Member>> GetAllMembersAsync()
    {
        return await _memberRepository.GetAllAsync();
    }

    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        return await _memberRepository.GetByIdAsync(id);
    }

    public async Task<Member> CreateMemberAsync(Member member)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Business logic: Set default values
            member.CreatedAt = DateTime.UtcNow;
            member.UpdatedAt = DateTime.UtcNow;
            member.IsActive = true;
            member.MembershipStatus = MembershipStatus.Active;
            
            // Business logic: Validate phone number uniqueness
            if (!string.IsNullOrEmpty(member.Phone) && !await _memberRepository.IsPhoneUniqueAsync(member.Phone))
            {
                throw new InvalidOperationException($"Member with phone number '{member.Phone}' already exists.");
            }

            // Add member to repository
            await _memberRepository.AddAsync(member);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            return member;
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            await transaction.RollbackAsync();
            
            switch (pgEx.SqlState)
            {
                case "23505": // Unique constraint violation
                    if (pgEx.ConstraintName?.Contains("Phone") == true)
                    {
                        throw new InvalidOperationException($"Member with phone number '{member.Phone}' already exists.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Duplicate entry violates unique constraint: {pgEx.ConstraintName}");
                    }
                
                case "23503": // Foreign key constraint violation
                    if (pgEx.ConstraintName?.Contains("UserId") == true)
                    {
                        throw new InvalidOperationException($"User with ID {member.UserId} does not exist.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Referenced record does not exist: {pgEx.ConstraintName}");
                    }
                
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

    public async Task<Member> UpdateMemberAsync(Member member)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingMember = await _memberRepository.GetByIdAsync(member.Id);
            if (existingMember == null)
            {
                throw new InvalidOperationException($"Member with ID {member.Id} not found.");
            }

            // Business logic: Validate phone number uniqueness (excluding current member)
            if (!string.IsNullOrEmpty(member.Phone) && !await _memberRepository.IsPhoneUniqueAsync(member.Phone, member.Id))
            {
                throw new InvalidOperationException($"Member with phone number '{member.Phone}' already exists.");
            }

            // Business logic: Update only allowed fields
            existingMember.FirstName = member.FirstName;
            existingMember.LastName = member.LastName;
            existingMember.Phone = member.Phone;
            existingMember.Address = member.Address;
            existingMember.DateOfBirth = member.DateOfBirth;
            existingMember.MembershipStatus = member.MembershipStatus;
            existingMember.UpdatedAt = DateTime.UtcNow;

            // Update member in repository
            await _memberRepository.UpdateAsync(existingMember);
            
            // Save changes within transaction
            await _context.SaveChangesAsync();
            
            // Commit transaction
            await transaction.CommitAsync();
            
            return existingMember;
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            await transaction.RollbackAsync();
            
            switch (pgEx.SqlState)
            {
                case "23505": // Unique constraint violation
                    if (pgEx.ConstraintName?.Contains("Phone") == true)
                    {
                        throw new InvalidOperationException($"Member with phone number '{member.Phone}' already exists.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Duplicate entry violates unique constraint: {pgEx.ConstraintName}");
                    }
                
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

    public async Task DeleteMemberAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
            {
                throw new InvalidOperationException($"Member with ID {id} not found.");
            }

            // Business logic: Check if member can be deleted (no active loans)
            var activeLoans = await _memberRepository.GetActiveLoansCountAsync(id);
            
            if (activeLoans > 0)
            {
                throw new InvalidOperationException($"Cannot delete member with ID {id}. They have {activeLoans} active loan(s).");
            }

            // Business logic: Soft delete instead of hard delete
            member.IsActive = false;
            member.MembershipStatus = MembershipStatus.Suspended;
            member.UpdatedAt = DateTime.UtcNow;

            // Update member in repository
            await _memberRepository.UpdateAsync(member);
            
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

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        return await _memberRepository.GetActiveMembersAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersWithUserInfoAsync()
    {
        return await _memberRepository.GetMembersWithUserInfoAsync();
    }

    public async Task<Member?> GetMemberByUserIdAsync(int userId)
    {
        return await _memberRepository.GetMemberByUserIdAsync(userId);
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone, int? excludeId = null)
    {
        return await _memberRepository.IsPhoneUniqueAsync(phone, excludeId);
    }

    public async Task<int> GetActiveLoansCountAsync(int memberId)
    {
        return await _memberRepository.GetActiveLoansCountAsync(memberId);
    }

    public async Task<IEnumerable<Member>> GetMembersWithActiveLoansAsync()
    {
        return await _memberRepository.GetMembersWithActiveLoansAsync();
    }
} 