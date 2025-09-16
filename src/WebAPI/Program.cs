using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Context;
using WebAPI.Entity;
using WebAPI.Middlewares;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Not: Middleware yada Pipileine i�in Transient kullanal�m
builder.Services.AddTransient<CustomErrorHandlingMiddleware>();

// Net Core uygulamalar�nda IoC Authentication Servis y�klenmeli

builder.Services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();


builder.Services.AddAuthentication(x =>
{
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
  opt.RequireHttpsMetadata = true;
  opt.SaveToken = true;
  opt.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("tGXQtanyYaJCxI3BseFJ3YZyEz_1m0twz0OcG6caJ8O4AraeWzhSR6X3TwC3xKTYGrxj8wshZ2MPz_lZU2oQPg")),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    LifetimeValidator = (notbefore, expires, securityToken, validationParamaters) =>
    {
      Console.Out.WriteLineAsync("LifetimeValidator Event");
      return expires != null && expires.Value > DateTime.UtcNow;
    }
  };

  // genelde bu k�s�mda g�venlik loglar� atars�z ki jwt s�recinde bir problem yada dikkat �eken bir durum ger�ekle�iyor mu - brute force sald�r�s� varsa loglardan anla��labilir.
  opt.Events = new JwtBearerEvents()
  {
    OnAuthenticationFailed = c =>
    {
      // Serilog
      Console.Out.WriteLineAsync("Authentication Failed" + c.Exception.Message);
      return Task.CompletedTask;
    },
    OnTokenValidated = c =>
    {
      Console.Out.WriteLineAsync("Authentication Valiated" + c.Result);
      return Task.CompletedTask;
    },
    OnForbidden = c =>
    {
      Console.Out.WriteAsync("Yetki Yok" + c.Principal?.Identity?.Name);
      return Task.CompletedTask;
    }
  };
});



builder.Services.AddDbContext<AppDbContext>(opt =>
{
  opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConn"));
});

builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // T�m requestlerde authentication kontrol et
app.UseAuthorization();

// Custom Middlewares

//app.Use(async (context, next) =>
//{
//  await next();

//});

app.UseMiddleware<CustomErrorHandlingMiddleware>();



app.MapControllers();

app.Run();
