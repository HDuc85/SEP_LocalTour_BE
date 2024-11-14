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
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

//add
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowSpecificOrigins",
      builder => builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigins");
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

app.Run();
