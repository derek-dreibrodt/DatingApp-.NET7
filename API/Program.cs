using API.Data;
using API.Data.Migrations;
using API.Entities;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddApplicationServices(builder.Configuration);// Uses extension in Extensions/ApplicationServiceExtension for app-related services
builder.Services
    .AddIdentityServices(builder.Configuration); // Uses extension in Extensions/IdentityServiceExtension for identity-related services

var app = builder.Build();

// middleware operates different based on environment, determined in launchSettings.json
app.UseMiddleware<ExceptionMiddleWare>(); // Configure middleware for using custom exception jsons

app.UseHttpsRedirection();

app.UseAuthentication(); // Do you have a valid token
app.UseAuthorization(); // now that you have a token, what are you allowed to do

// x
app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.MapControllers();


// Starts use of a scope, which doesn't happen in httpcontext pipeline 
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider; // -> this allows scope of the services of the app


try
{
    var context = services.GetRequiredService<DataContext>(); // Add the DbContext service to the scope of this try block in "context" var
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync(); // Creates a clean database if it is deleted/doesn't have migrations applied - removes need for 'dotnet ef migrations add {name}'
    await Seed.SeedUsers(userManager, roleManager); // calls seed user upon creation
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>(); // get the logger service into the scope and log an error if there is one
    logger.LogError(ex, "An error occurred during migration and seeding");
}

app.Run();
