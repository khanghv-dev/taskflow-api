using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoSecretController : ControllerBase
{
    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetMyProfile()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            message = "Xac thuc thanh cong!",
            currentUser = username,
            role = role
        });
    }
}