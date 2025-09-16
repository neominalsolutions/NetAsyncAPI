using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using WebAPI.Dtos;

namespace WebAPI.Services
{
  public class JsonWebTokenService : IJsonWebTokenService
  {
    public const string SecretKey = "tGXQtanyYaJCxI3BseFJ3YZyEz_1m0twz0OcG6caJ8O4AraeWzhSR6X3TwC3xKTYGrxj8wshZ2MPz_lZU2oQPg";

    private const double EXPIRE_MINUTES = 15;
    public TokenResponse GenerateTokenAsync(ClaimsIdentity identity, CancellationToken token)
    {
      var key = Encoding.ASCII.GetBytes(SecretKey);
      var tokenHandler = new JwtSecurityTokenHandler();
      var descriptor = new SecurityTokenDescriptor
      {
        Subject = identity,
        Expires = DateTime.UtcNow.AddMinutes(EXPIRE_MINUTES),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512),
      };
      SecurityToken tk = tokenHandler.CreateToken(descriptor);
      var accessToken = tokenHandler.WriteToken(tk);

      return new TokenResponse(accessToken);

    }
  }
}
