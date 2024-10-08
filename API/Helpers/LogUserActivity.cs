using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
  public class LogUserActivity : IAsyncActionFilter
  {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      ActionExecutedContext resultContext = await next();

      if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

      int userId = resultContext.HttpContext.User.GetUserId();
      IUserRepository? repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
      AppUser user = await repo.GetUserByIdAsync(userId);
      user.LastActiveAt = DateTime.Now;
      await repo.SaveAllAsync();
    }
  }
}