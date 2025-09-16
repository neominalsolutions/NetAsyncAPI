using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Dtos;
using WebAPI.Entity;
using WebAPI.Services;

namespace WebAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountsController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly IJsonWebTokenService _jsonWebTokenService;


    public AccountsController(UserManager<AppUser> userManager, IJsonWebTokenService jsonWebTokenService)
    {
      _userManager = userManager;
      _jsonWebTokenService = jsonWebTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {

      var user = new AppUser();
      user.Email = registerDto.Username;
      user.UserName = registerDto.Username;

      // şifreli bir şekilde parola oluşturur.
   
      var response =  await _userManager.CreateAsync(user, registerDto.Password);

      //if(response.IsCompletedSuccessfully)
      //{
      //  return Ok("Register işlemi başarılı");
      //}

      if (response.Succeeded)
      {
        return Ok("Register işlemi başarılı");
      }

      return BadRequest();
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {

      var user = await _userManager.FindByNameAsync(loginRequestDto.UserName); 

      var passwordCheck = await _userManager.CheckPasswordAsync(user,loginRequestDto.Password);

      if(passwordCheck)
      {

        var claims = new List<Claim>();
        claims.Add(new Claim("UserId", user.Id));
        claims.Add(new Claim("sub", user.UserName));
        claims.Add(new Claim(ClaimTypes.Role, "admin"));
        claims.Add(new Claim(ClaimTypes.Role, "manager"));
        claims.Add(new Claim("User", "Insert"));
        claims.Add(new Claim("User", "Update"));
        claims.Add(new Claim("User", "Approve"));
        claims.Add(new Claim("User", "Delete"));

        var claimsIdentity = new ClaimsIdentity(claims);

        var response = _jsonWebTokenService.GenerateTokenAsync(claimsIdentity, cancellationToken);

        return Ok(response);
      } 
      else
      {
        return BadRequest("Kimlik bilgileri hatalı");
      }
    }


  }
}
