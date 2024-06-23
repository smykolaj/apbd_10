using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using JWT.Models;
using JWT.DTOs;
using JWT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Controllers;

[Route("api/[controller]")]
public class ExamplesController(IConfiguration config, IDbService service) : ControllerBase
{

    
    [HttpGet("hash-password-without-salt/{password}")]
    public IActionResult HashPassword(string password)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            new byte[] { 0 },
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
        return Ok(passwordHasher.VerifyHashedPassword(new User(), requestModel.Hash, requestModel.Password) ==
                  PasswordVerificationResult.Success);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequestModel model)
    {
        try
        { 
            LoginResponseModel answer = service.Login(model);
            return Ok(answer);
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }

       
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult RegisterStudent(RegisterRequest model)
    {
        try
        {
            service.RegisterUser(model);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
        
    }

    [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
    [HttpPost("refresh")]
    public IActionResult Refresh(string refreshToken)
    {
        LoginResponseModel responseModel;
        try
        {
            responseModel = service.Refresh(refreshToken);
            return Ok(responseModel);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}


