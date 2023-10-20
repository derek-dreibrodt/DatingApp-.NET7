using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        

        public static void AddIdentityServices(this IServiceCollection services, 
        IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt => 
            {
                opt.Password.RequireNonAlphanumeric = false;
                //opt.User.RequireUniqueEmail;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>() // Configures RoleManager service
                .AddEntityFrameworkStores<DataContext>(); // Creates all the tables related to identity in our database
            // Authentication always comes before authorization
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer (options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding
                    .UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin")); //another way to add authorization
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });
        }
    }
}