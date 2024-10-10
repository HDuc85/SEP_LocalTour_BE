using LocalTour.Data;
using LocalTour.Data.Abstract;
using LocalTour.Domain.Entities;
using LocalTour.Services.Abstract;
using LocalTour.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace LocalTour.Infrastructure.Configuration
{
    public static class ConfigurationService
    {
        public static void RegesterContextDb(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<LocalTourDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("LocalTourDBConnection")));
            service.AddIdentityCore<User>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
            })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<LocalTourDbContext>();
            service.AddScoped<PasswordHasher<User>>();

        }

        public static void RegesterDI(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            service.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            service.AddScoped<IUserService, UserService>();
            service.AddScoped<ITokenHandler, Services.Services.TokenHandler>();

        }

        public static void RegesterIdentity(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddIdentityCore<User>(options =>
            {
               // options.SignIn.RequireConfirmedPhoneNumber = true;
            }).AddEntityFrameworkStores<LocalTourDbContext>();
        }

        public static void RegesterTokenBearer(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
              ).AddJwtBearer(options =>
              {
                  options.IncludeErrorDetails = true;
                  options.SaveToken = true;
                  options.UseSecurityTokenValidators = true;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      ValidateAudience = false,
                      ValidateIssuer = false,
                      ValidateLifetime = true,
                      ValidIssuer = configuration["JWT:Issuer"],
                      ValidAudience = configuration["JWT:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),
                      ClockSkew = TimeSpan.Zero
                  };
                  options.Events = new JwtBearerEvents()
                  {
                      OnTokenValidated = context =>
                      {
                          var tokenHandler = context.HttpContext.RequestServices.GetRequiredService<ITokenHandler>();
                          return tokenHandler.ValidateToken(context);
                      },
                      OnAuthenticationFailed = context =>
                      {
                          return Task.CompletedTask;
                      },
                      OnMessageReceived = context =>
                      {


                          return Task.CompletedTask;
                      },
                      OnChallenge = context =>
                      {
                          return Task.CompletedTask;
                      },
                  };
              });

        }


    }
}
