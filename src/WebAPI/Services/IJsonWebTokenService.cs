using System.Security.Claims;
using WebAPI.Dtos;

namespace WebAPI.Services
{
  public interface IJsonWebTokenService
  {
    TokenResponse GenerateTokenAsync(ClaimsIdentity identity,CancellationToken token);
  }
}
