using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SingalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
      services.AddSingleton<PresenceTracker>();
      services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IPhotoService, PhotoService>();
      services.AddScoped<LogUserActivity>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();

      services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

      services.AddDbContext<DataContext>(options =>
      {
        options.UseSqlite(config.GetConnectionString("DefaultConnection"));
      });

      return services;
    }
  }
}