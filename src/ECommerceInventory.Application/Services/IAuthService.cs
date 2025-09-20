using ECommerceInventory.Application.DTOs;

namespace ECommerceInventory.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    string GenerateJwtToken(string email, string username, int userId);
}
