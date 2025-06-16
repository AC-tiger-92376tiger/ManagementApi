using ManagementApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


namespace ManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _UserContext;
        public UserController(ApplicationDbContext UserContext)
        {
            _UserContext = UserContext;
        }
        /*
                [HttpGet]
                public async Task<IActionResult> GetUser(
                   string? search,
                   string? sort = "name",
                   string? order = "asc",
                   int page = 1,
                   int pageSize = 10)
                {
                    var query = _UserContext.Users.AsQueryable();

                    // Search
                    if (!string.IsNullOrEmpty(search))
                    {
                        query = query.Where(p => p.Username.Contains(search));
                    }

                    // Sorting
                    query = (sort.ToLower(), order.ToLower()) switch
                    {
                        ("username", "asc") => query.OrderBy(p => p.Username),
                        ("username", "desc") => query.OrderByDescending(p => p.Username),
                        ("email", "asc") => query.OrderBy(p => p.Email),
                        ("email", "desc") => query.OrderByDescending(p => p.Email),
                        _ => query.OrderBy(p => p.Id)
                    };

                    // Pagination
                    var total = await query.CountAsync();
                    var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                    /*
                                return Ok(new
                                {
                                    total,
                                    page,
                                    pageSize,
                                    data = products
                                });
                    return Ok(total);
                }
        */
        [HttpGet]
        public async Task<IActionResult> GetStudent()
        {
            var student = await _UserContext.Users.ToListAsync();
            return Ok(student);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var dbUser = await _UserContext.Users.FindAsync(id);
            return Ok(dbUser);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User User)
        {
            if (User == null)
            {
                return BadRequest("Not found");
            }

            _UserContext.Users.Add(User);
            await _UserContext.SaveChangesAsync();
            return Ok(await _UserContext.Users.ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User User)
        {
            var dbUser = await _UserContext.Users.FindAsync(id);
            if (dbUser == null)
            {
                return BadRequest("User not found");
            }
        
            dbUser.Username = User.Username;
            dbUser.Email = User.Email;
            dbUser.Role = User.Role;
            dbUser.PasswordHash = GenerateJwtToken(User.PasswordHash);
            dbUser.CreatedAt = DateTime.UtcNow;

            await _UserContext.SaveChangesAsync();
            return Ok(await _UserContext.Users.ToListAsync());

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var dbUser = await _UserContext.Users.FindAsync(id);
            if (dbUser == null)
            {
                return BadRequest("User not found");
            }

            _UserContext.Users.Remove(dbUser);
            await _UserContext.SaveChangesAsync();
            return Ok(await _UserContext.Users.ToListAsync());
        }
        public string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
