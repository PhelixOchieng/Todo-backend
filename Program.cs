using System.Text;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
                 policy => policy.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader());
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
      string? secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
      if (secretKey == null)
        throw new InvalidOperationException(
            "The environment variable \"JWT_SECRET_KEY\" was not found.");

      options.TokenValidationParameters = new TokenValidationParameters() {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "todoApp",
        ValidAudience = "todoApp",
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
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

app.UseHttpLogging();
app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseCors(AllowAllOriginsPolicy);
  app.UseSwagger();
  app.UseSwaggerUI();
} else {
  app.UseCors(AllowSelectOriginsPolicy);
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ResponseFormattingMiddleware>();
app.MapControllers();

app.Run();
