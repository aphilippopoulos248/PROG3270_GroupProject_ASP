using PROG3270_GroupProject.Domain.Entities;
using PROG3270_GroupProject.Models;

namespace PROG3270_GroupProject.Infrastructure.Interfaces
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllAsync();
        Task AddAsync(Member member);
        Task<Member> AuthenticateAsync(Login login);
    }
}
