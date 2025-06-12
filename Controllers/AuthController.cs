using Microsoft.AspNetCore.Mvc;
using ManagementApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registeruser)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registeruser.Email))
            {
                return BadRequest(new { message = "UserEmail already exists" });
            }
            // Generate CreatedAt timestamp
            //user.CreatedAt = DateTime.UtcNow;
            // Hash the password before saving
            registeruser.Password = BCrypt.Net.BCrypt.HashPassword(registeruser.Password);
            var userEntity = new User
            {
                Email = registeruser.Email,
                PasswordHash = registeruser.Password,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(user.Email);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string username)
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

    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}