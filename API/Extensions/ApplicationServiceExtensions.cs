using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        // Services are good for things that we use in multiple parts of our application
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
        IConfiguration config)
        {
            services.AddDbContext<DataContext>(opts => 
            {
                opts.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddCors();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            // Add the token service
            services.AddScoped<ITokenService, TokenService>();


            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add the cloudinary credentials - gets section from CloudinarySettings of appsettings.json and puts them into a CloudinarySettings class
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();

            // Add service for action filter for logging users' activity
            services.AddScoped<LogUserActivity>();

            // Add repository service for likes
            services.AddScoped<ILikesRepository, LikesRepository>();

            // Add repository service for messaging
            services.AddScoped<IMessageRepository, MessageRepository>();



            return services;
        }
    }
}