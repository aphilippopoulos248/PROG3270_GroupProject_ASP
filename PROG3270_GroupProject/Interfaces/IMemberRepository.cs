using PROG3270_GroupProject.Domain.Entities;

namespace PROG3270_GroupProject.Interfaces
{
    public interface IMemberRepository
    {
        Task<Member> GetMemberByCredentialsAsync(string username, string password);
        Task<Member> GetMemberByIdAsync(int id);
        Task<Member> AddMemberAsync(Member member);
    }
};