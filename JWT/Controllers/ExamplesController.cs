using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Controllers;

[Route("api/[controller]")]
public class ExamplesController(IConfiguration config) : ControllerBase
{
    [HttpGet("hash-password-without-salt/{password}")]
    public IActionResult HashPassword(string password)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            new byte[] {0},
            10,
            HashAlgorithmName.SHA512,
            128
        );

        return Ok(Convert.ToHexString(hash));
    }
    
    [HttpGet("hash-password/{password}")]
    public IActionResult HashPasswordWithSalt(string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return Ok(passwordHasher.HashPassword(new User(), password));
    }
    
    [HttpPost("verify-password")]
    public IActionResult VerifyPassword(VerifyPasswordRequestModel requestModel)
    {
        var passwordHasher = new PasswordHasher<User>();
        return Ok(passwordHasher.VerifyHashedPassword(new User(), requestModel.Hash, requestModel.Password) == PasswordVerificationResult.Success);
    }
    
    [HttpPost("login")]
    public IActionResult Login(LoginRequestModel model) 
    {

        if(!(model.UserName.ToLower() == "test" && model.Password == "test"))
        {
            return Unauthorized("Wrong username or password");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescription = new SecurityTokenDescriptor()
        {
            Issuer = config["JWT:Issuer"],
            Audience = config["JWT:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!)),
                SecurityAlgorithms.HmacSha256
            )
        };
        var token = tokenHandler.CreateToken(tokenDescription);
        var stringToken = tokenHandler.WriteToken(token);

        var refTokenDescription = new SecurityTokenDescriptor
        {
            Issuer = config["JWT:RefIssuer"],
            Audience = config["JWT:RefAudience"],
            Expires = DateTime.UtcNow.AddDays(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:RefKey"]!)),
                SecurityAlgorithms.HmacSha256
            )
        };
        var refToken = tokenHandler.CreateToken(refTokenDescription);
        var stringRefToken = tokenHandler.WriteToken(refToken);
        return Ok(new LoginResponseModel
        {
            Token = stringToken,
            RefreshToken = stringRefToken
        });
    }
    
    [HttpPost("refresh")]
    public IActionResult RefreshToken(RefreshTokenRequestModel requestModel)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(requestModel.RefreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["JWT:RefIssuer"],
                ValidAudience = config["JWT:RefAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:RefKey"]!))
            }, out SecurityToken validatedToken);
            return Ok(true + " " + validatedToken);
        }
        catch
        {
            return Unauthorized();
        }
    }
}

public class User
{
    public string Name { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class VerifyPasswordRequestModel
{
    public string Password { get; set; } = null!;
    public string Hash { get; set; } = null!;
}

public class LoginRequestModel
{
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}

public class LoginResponseModel
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenRequestModel
{
    public string RefreshToken { get; set; } = null!;
}
