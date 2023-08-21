using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // context is the action that is about to be executed
            var resultContext = await next(); // Gives us the action executed context - AFTER THE API HAS COMPLETED

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return; // if user is not authenticated, return

            var userId = resultContext.HttpContext.User.GetUserId();
            Console.WriteLine("Hello");
            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(int.Parse(userId));
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllAsync();
        }
    }
}