using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthDemo.Api.Data;
using AuthDemo.Api.DTOs;
using AuthDemo.Api.Models;
using AuthDemo.Api.Services;

namespace AuthDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // 1. Kiểm tra tài khoản đã tồn tại chưa
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });
        }

        // 2. Băm mật khẩu (Hash) an toàn bằng BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Lưu vào Database
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đăng ký tài khoản thành công!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Tìm user theo Username
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Tên tài khoản hoặc mật khẩu không chính xác." });
        }

        // 2. So khớp mật khẩu đã băm
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Tên tài khoản hoặc mật khẩu không chính xác." });
        }

        // 3. Đúng mật khẩu -> Cấp JWT Token
        var token = _tokenService.CreateToken(user);

        return Ok(new AuthResponse(token, user.Username, user.Role));
    }
}