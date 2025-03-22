using PROG3270_GroupProject.Application.Interfaces;
using PROG3270_GroupProject.Domain.Entities;
using PROG3270_GroupProject.Infrastructure.Interfaces;
using PROG3270_GroupProject.Infrastructure.Repositories;
using PROG3270_GroupProject.Models;


namespace PROG3270_GroupProject.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }
        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            return await _memberRepository.GetAllAsync();
        }
        public async Task AddMemberAsync(Member member)
        {
            await _memberRepository.AddAsync(member);
        }
        public async Task<Member> GetMemberByCredentials(Login login)
        {
            return await _memberRepository.AuthenticateAsync(login);
        }
    }
}
