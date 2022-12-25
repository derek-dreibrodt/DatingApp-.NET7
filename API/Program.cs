using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// middleware operates different based on environment, determined in launchSettings.json
app.UseMiddleware<ExceptionMiddleWare>(); // Configure middleware for using custom exception jsons

app.UseHttpsRedirection();

app.UseAuthentication(); // Do you have a valid token
app.UseAuthorization(); // now that you have a token, what are you allowed to do

// x
app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.MapControllers();

app.Run();
