using System.Globalization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using LocalTour.Infrastructure.Configuration;
using Microsoft.OpenApi.Models;
using Service.Common.Mapping;
using System.Reflection;
using LocalTour.WebApi.Middleware;
using LocalTour.Services.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegesterContextDb(builder.Configuration);
builder.Services.RegesterDI(builder.Configuration);
builder.Services.RegesterIdentity(builder.Configuration);
builder.Services.RegesterTokenBearer(builder.Configuration);
builder.Services.RegesterPayOS(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
//add
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowSpecificOrigins", policy =>
  {
    policy.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

builder.Services.AddSwaggerGen(options =>
{
  options.UseDateOnlyTimeOnlyStringConverters();
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Type = SecuritySchemeType.Http,
    In = ParameterLocation.Header,
    BearerFormat = "JWT",
    Scheme = "Bearer",
    Description = "Input only token"
  });
  options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },


            },
            new List<string>()
          }
        });


  /*   var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
     options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));*/

});

FirebaseApp.Create(new AppOptions()
{
  Credential = GoogleCredential.FromFile("firebaseServiceAccount.json"),
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build(); app.UseCors("AllowSpecificOrigins");
  


var defaultCulture = new CultureInfo("en-US"); // Thay bằng mã văn hóa mong muốn
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

app.MapGet("/culture-info", () => new
{
  CurrentCulture = CultureInfo.CurrentCulture.Name,
  DateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern,
  DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
});

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

//Middleware
app.UseMiddleware<CheckUserBanMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(); 
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media")),
  RequestPath
    = "/Meida"
});

app.MapControllers();
app.Use(async (context, next) =>
{
  context.Response.OnStarting(() =>
  {
    var headers = context.Response.Headers;
    if (headers.ContainsKey("Access-Control-Allow-Origin"))
    {
      Console.WriteLine($"CORS Header: {headers["Access-Control-Allow-Origin"]}");
    }
    else
    {
      Console.WriteLine("CORS Header not set.");
    }
    return Task.CompletedTask;
  });
  await next();
});
app.Run();
