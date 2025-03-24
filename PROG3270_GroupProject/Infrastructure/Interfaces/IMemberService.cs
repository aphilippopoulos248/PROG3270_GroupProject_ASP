using PROG3270_GroupProject.Domain.Entities;
using PROG3270_GroupProject.Models;

namespace PROG3270_GroupProject.Application.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<Member>> GetAllMembersAsync();
        Task AddMemberAsync(Member member);
        Task<Member> GetMemberByCredentials(Login login);
    }
}
