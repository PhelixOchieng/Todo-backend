using Microsoft.AspNetCore.HttpLogging;

using Todo.Context;
using Todo.Filters;

// Load .env* environment variables
var rootDir = Directory.GetCurrentDirectory();
DotEnv.Load(rootDir);

var builder = WebApplication.CreateBuilder(args);

// Setup cors
string AllowAllOriginsPolicy = "_AllowAllOrigins";
string AllowSelectOriginsPolicy = "_AllowSelectOrigins";
builder.Services.AddCors(opts =>
{
    opts.AddPolicy(
        name: AllowAllOriginsPolicy,
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
    opts.AddPolicy(
        name: AllowSelectOriginsPolicy,
        policy => policy.WithOrigins("https://todos.com")
    );
});

// Add controller service
builder.Services.AddControllers(opts => opts.Filters.Add<GlobalResponseFilter>());

// Add DB service
builder.Services.AddDbContext<TodoContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure request logging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields =
        HttpLoggingFields.RequestPath
        | HttpLoggingFields.RequestMethod
        | HttpLoggingFields.RequestProtocol
        | HttpLoggingFields.ResponseStatusCode;
});

var app = builder.Build();
app.UseHttpLogging();
app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(AllowAllOriginsPolicy);
}
else
{
    app.UseCors(AllowAllOriginsPolicy);
}

app.UseAuthorization();
app.MapControllers();

app.Run();
