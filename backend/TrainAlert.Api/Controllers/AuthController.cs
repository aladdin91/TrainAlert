using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TrainAlert.Api.Services;

namespace TrainAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly AuthService _authService;
  private readonly IConfiguration _configuration;

  public AuthController(
      AuthService authService,
      IConfiguration configuration)
  {
    _authService = authService;
    _configuration = configuration;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(
      RegisterRequest request)
  {
    var user = await _authService.RegisterAsync(
        request.Email,
        request.Password);

    if (user is null)
    {
      return Conflict(new
      {
        message = "Email is already registered."
      });
    }

    return Ok(new
    {
      user.Id,
      user.Email
    });
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(
      LoginRequest request)
  {
    var user =
        await _authService.ValidateCredentialsAsync(
            request.Email,
            request.Password);

    if (user is null)
    {
      return Unauthorized(new
      {
        message = "Invalid email or password."
      });
    }

    var token = GenerateToken(
        user.Id,
        user.Email);

    return Ok(new
    {
      token,
      user = new
      {
        user.Id,
        user.Email
      }
    });
  }

  private string GenerateToken(
      Guid userId,
      string email)
  {
    var key =
        _configuration["Jwt:Key"]
        ?? throw new InvalidOperationException(
            "JWT key is not configured.");

    var issuer =
        _configuration["Jwt:Issuer"];

    var audience =
        _configuration["Jwt:Audience"];

    var claims = new[]
    {
    new Claim(
        ClaimTypes.NameIdentifier,
        userId.ToString()),

    new Claim(
        ClaimTypes.Email,
        email)
};

    var credentials =
        new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(7),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler()
        .WriteToken(token);
  }
}

public record RegisterRequest(
    string Email,
    string Password);

public record LoginRequest(
    string Email,
    string Password);