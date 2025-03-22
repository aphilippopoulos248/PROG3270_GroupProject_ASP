using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Domain.Entities;
using PROG3270_GroupProject.Infrastructure.Data;
using PROG3270_GroupProject.Infrastructure.Interfaces;
using PROG3270_GroupProject.Models;



namespace PROG3270_GroupProject.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ProjectContext _context;

        public MemberRepository(ProjectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Member>> GetAllAsync()
        {
            var members = await _context.Members.ToListAsync();
            return members;
        }

        public async Task AddAsync(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
        }
        public async Task<Member> AuthenticateAsync( Login login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                throw new ArgumentException("Invalid login credentials.");
            }

            // Check if member exists with matching username and password
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.UserName == login.Username && m.Password == login.Password);

            if (member == null)
            {
                throw new KeyNotFoundException("Employee not found with the provided credentials.");
            }

            return member;
        }
    }
}
