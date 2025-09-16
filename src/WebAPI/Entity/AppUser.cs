using Microsoft.AspNetCore.Identity;

namespace WebAPI.Entity
{
  public class AppUser : IdentityUser<string>
  {

    public AppUser()
    {
      Id = Guid.NewGuid().ToString();
    }
  }
}
