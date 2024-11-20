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
using Service.Common.Mapping;
using System.Text;
using Quartz;
using Quartz.Impl;

namespace LocalTour.Infrastructure.Configuration
{
    public static class ConfigurationService
    {
        public static void RegesterContextDb(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<LocalTourDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("LocalTourDBConnection")),ServiceLifetime.Scoped);
            service.AddIdentityCore<User>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = false;

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
            service.AddScoped<IFileService, FileService>();
            service.AddScoped<IFollowUserService, FollowUserService>();
            service.AddScoped<ITraveledPlaceService, TraveledPlaceService>();
            service.AddScoped<IMarkPlaceService, MarkPlaceService>();
            service.AddScoped<IPlaceSearchHistoryService, PlaceSearchHistoryService>();
            service.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            service.AddSingleton(provider =>
            {
                var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
                return schedulerFactory.GetScheduler().Result;
            });
            service.AddScoped<INotificationService, NotificaitonService>();

            service.AddAutoMapper(typeof(MappingProfile));

            service.AddScoped<IPostService, PostService>();
            service.AddScoped<IPostMediumService, PostMediumService>();
            service.AddScoped<IPostCommentService, PostCommentService>();
            service.AddScoped<IPostCommentLikeService, PostCommentLikeService>();
            service.AddScoped<IPostLikeService, PostLikeService>();

            service.AddScoped<IEventService, EventService>();
            service.AddScoped<IPlaceActivityService, PlaceActivityService>();
            service.AddScoped<IPlaceService, PlaceService>();
            service.AddScoped<IPlaceFeedbackService, PlaceFeedbackService>();


            service.AddScoped<IScheduleService, ScheduleService>();
            service.AddScoped<IDestinationService, DestinationService>();

            service.AddScoped<IScheduleLikeService, ScheduleLikeService>();

            service.AddScoped<IUserReportService, UserReportService>();
            service.AddScoped<IPlaceReportService, PlaceReportService>();
            service.AddScoped<IUserPreferenceTagsService, UserPreferenceTagsService>();
            service.AddScoped<IModTagService, ModTagService>();


            service.AddScoped<IPlaceFeedbackHelpfulService, PlaceFeedbackHelpfulService>();
            service.AddScoped<ITagService, TagService>();
        }

        public static void RegesterIdentity(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddIdentityCore<User>(options =>
            {
                // options.SignIn.RequireConfirmedPhoneNumber = true;
            }).AddEntityFrameworkStores<LocalTourDbContext>();

            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            service.AddHttpContextAccessor();
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

        public static void RegesterQuazrtz(this IServiceCollection service)
        {
            service.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
            });
            service.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }

    }

}
