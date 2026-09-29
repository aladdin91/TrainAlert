using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainAlert.Api.Data;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class AuthService
{
  private readonly ApplicationDbContext _db;
  private readonly PasswordHasher<User> _passwordHasher;

  public AuthService(
      ApplicationDbContext db,
      PasswordHasher<User> passwordHasher)
  {
    _db = db;
    _passwordHasher = passwordHasher;
  }

  public async Task<User?> RegisterAsync(
      string email,
      string password)
  {
    email = email.Trim().ToLowerInvariant();

    var existingUser = await _db.Users
        .FirstOrDefaultAsync(user => user.Email == email);

    if (existingUser is not null)
    {
      return null;
    }

    var user = new User
    {
      Id = Guid.NewGuid(),
      Email = email,
      CreatedAt = DateTime.UtcNow
    };

    user.PasswordHash =
        _passwordHasher.HashPassword(
            user,
            password);

    _db.Users.Add(user);

    await _db.SaveChangesAsync();

    return user;
  }

  public async Task<User?> ValidateCredentialsAsync(
      string email,
      string password)
  {
    email = email.Trim().ToLowerInvariant();

    var user = await _db.Users
        .FirstOrDefaultAsync(user => user.Email == email);

    if (user is null)
    {
      return null;
    }

    var result =
        _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

    if (result == PasswordVerificationResult.Failed)
    {
      return null;
    }

    return user;
  }
}