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
        //private readonly ProjectContext _context;
        private readonly IMemberService _memberService;

        //public MemberController(ProjectContext context)
        //{
        //    _context = context;
        //}

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            //var members = await _context.Members.ToListAsync();
            //return Ok(members);
            var members = await _memberService.GetAllMembersAsync();
            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> PostMember([FromBody] Member member)
        {
            //if (member == null)
            //{
            //    return BadRequest("Invalid data.");
            //}

            //_context.Members.Add(member);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(PostMember), new { id = member.MemberID }, member);
            await _memberService.AddMemberAsync(member);
            return CreatedAtAction(nameof(GetMembers), new { id = member.MemberID }, member);
        }
        [HttpPost("authenticate")]
        public async Task<ActionResult<Member>> GetMemberByCredentials([FromBody] Login login)
        {
            //if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            //{
            //    return BadRequest("Invalid login credentials.");
            //}

            //// Check if member exists with matching username and password
            //var member = await _context.Members
            //    .FirstOrDefaultAsync(m => m.UserName == login.Username && m.Password == login.Password);

            //if (member == null)
            //{
            //    return NotFound("Employee not found with the provided credentials.");
            //}

            //return Ok(member);
            if (login == null)
            {
                return BadRequest("Invalid login request.");
            }

            try
            {
                // Authenticate the user using the service
                var member = await _memberService.GetMemberByCredentials(login);
                return Ok(member); // Return member or a token, depending on your implementation
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return Unauthorized(ex.Message); // Could return unauthorized if invalid credentials
            }
        }
    }
}
