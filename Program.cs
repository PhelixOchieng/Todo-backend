using System.Text;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

using Application.Core.Context;
using Application.Core.Middlewares;
using Auth.Service;
using Users.Models;

// Load .env* environment variables
var rootDir = Directory.GetCurrentDirectory();
DotEnv.Load(rootDir);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddLogging();

// Add response formatting service
builder.Services.AddTransient<ResponseFormattingMiddleware>();

// Setup cors
string AllowAllOriginsPolicy = "_AllowAllOrigins";
string AllowSelectOriginsPolicy = "_AllowSelectOrigins";
builder.Services.AddCors(opts => {
  opts.AddPolicy(name: AllowAllOriginsPolicy,
                 policy =>
                     policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
  opts.AddPolicy(name: AllowSelectOriginsPolicy,
                 policy => policy.WithOrigins("https://todos.com"));
});

// AAdd controller service
builder.Services.AddControllers();

// Add DB service
builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
      options.TokenValidationParameters = new TokenValidationParameters() {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "todoApp",
        ValidAudience = "todoApp",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("!SomethingSecret!")),
      };
    });
builder.Services
    .AddIdentityCore<User>(options => {
      options.SignIn.RequireConfirmedAccount = false;
      options.User.RequireUniqueEmail = true;
      options.Password.RequireDigit = false;
      options.Password.RequiredLength = 6;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequireUppercase = false;
      options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<TokenService, TokenService>();

// Configure request logging
builder.Services.AddHttpLogging(logging => {
  logging.LoggingFields =
      HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod |
      HttpLoggingFields.RequestProtocol | HttpLoggingFields.ResponseStatusCode;
});

var app = builder.Build();
app.UseMiddleware<ResponseFormattingMiddleware>();

app.UseHttpLogging();
app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseCors(AllowAllOriginsPolicy);
} else {
  app.UseCors(AllowAllOriginsPolicy);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
