using Microsoft.AspNetCore.Mvc;
using PROG3270_GroupProject.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Models;

namespace PROG3270_GroupProject.Controllers
{
    [Route("api/members")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ProjectContext _context;

        public MemberController(ProjectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var members = await _context.Members.ToListAsync();
            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> PostMember([FromBody] Member member)
        {
            if (member == null)
            {
                return BadRequest("Invalid data.");
            }

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostMember), new { id = member.MemberID }, member);
        }
        [HttpPost("authenticate")]
        public async Task<ActionResult<Member>> GetMemberByCredentials([FromBody] Login login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Invalid login credentials.");
            }

            // Check if member exists with matching username and password
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.UserName == login.Username && m.Password == login.Password);

            if (member == null)
            {
                return NotFound("Employee not found with the provided credentials.");
            }

            return Ok(member);
        }
    }
}
