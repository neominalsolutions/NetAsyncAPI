using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI.Entity;

namespace WebAPI.Context
{
  public class AppDbContext:IdentityDbContext<AppUser,AppRole,string>
  {
        public DbSet<Category> Categories { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opt):base(opt)
        {
            
        }
    }
}
