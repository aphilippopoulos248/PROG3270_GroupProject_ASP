using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Domain.Entities;
using PROG3270_GroupProject.Infrastructure.Data;
using PROG3270_GroupProject.Interfaces;
using PROG3270_GroupProject.Models;

namespace PROG3270_GroupProject.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly ProjectContext _context;
    private readonly ILogger<MemberRepository> _logger;

    public MemberRepository(ProjectContext context, ILogger<MemberRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Member> GetMemberByCredentialsAsync(string username, string password)
    {
        _logger.LogInformation($"Attempting to authenticate member: {username}");
            
        try
        {
            return await _context.Members
                .FirstOrDefaultAsync(m => m.UserName == username && m.Password == password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error authenticating member: {username}");
            throw;
        }
    }

    public async Task<Member> GetMemberByIdAsync(int id)
    {
        _logger.LogInformation($"Getting member with ID: {id}");
            
        try
        {
            return await _context.Members.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting member with ID: {id}");
            throw;
        }
    }

    public async Task<Member> AddMemberAsync(Member member)
    {
        _logger.LogInformation($"Adding new member: {member.UserName}");
            
        try
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding member: {member.UserName}");
            throw;
        }
    }
}