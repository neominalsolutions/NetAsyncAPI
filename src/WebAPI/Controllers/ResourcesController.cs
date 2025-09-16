using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ResourcesController : ControllerBase
  {

    // Token Request atmam lazım
    // JwtBearerDefaults.AuthenticationScheme Bu şemadan authorized olanlar gelebilir.

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public async Task<IActionResult> GetDataAsync()
    {
      return Ok("Only See Authenticated User");
    }

    // Jwt payload içerisinde role varsa buraya girer yoksa girmez.
    [Authorize(Roles = "admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CheckRoleAsync()
    {
      return Ok("Only Admin User");
    }



  }
}
