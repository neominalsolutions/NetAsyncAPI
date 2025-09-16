
namespace WebAPI.Middlewares
{
  public record ErrorResponse(string Message);

  // Uygulama genelinde tüm isteklerin bir try catch yapısında geçmesini sağlayan merkezi bir arayazılım yapıyoruz.
  public class CustomErrorHandlingMiddleware : IMiddleware
  {
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {

      try
      {
        await next(context);
      }
      catch (Exception ex)
      {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
      }
    }
  }
}
