using JWT.Controllers;
using JWT.DTOs;

namespace JWT.Services;

public interface IDbService
{
    void RegisterUser(RegisterRequest model);
    LoginResponseModel Login(LoginRequestModel model);
    string GenerateRefreshToken();
    string GetHashedPasswordWithSalt(string password, string salt);
    Tuple<string, string> GetHashedPasswordAndSalt(string password);

    LoginResponseModel Refresh(string refreshToken);
}