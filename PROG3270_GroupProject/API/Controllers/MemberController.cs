using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Models;
using PROG3270_GroupProject.Infrastructure.Data;
using PROG3270_GroupProject.Domain.Entities;
using PROG3270_GroupProject.Application.Interfaces;
using PROG3270_GroupProject.Application.Services;

namespace PROG3270_GroupProject.API.Controllers
{
    [ApiController]
    [Route("api/members")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var members = await _memberService.GetAllMembersAsync();
            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> PostMember([FromBody] Member member)
        {
            await _memberService.AddMemberAsync(member);
            return CreatedAtAction(nameof(GetMembers), new { id = member.MemberID }, member);
        }
        [HttpPost("authenticate")]
        public async Task<ActionResult<Member>> GetMemberByCredentials([FromBody] Login login)
        {
            if (login == null)
            {
                return BadRequest("Invalid login request.");
            }
            try
            {
                var member = await _memberService.GetMemberByCredentials(login);
                return Ok(member);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
