using Microsoft.AspNetCore.HttpLogging;

using Todo.Context;
using Todo.Filters;

// Load .env* environment variables
var rootDir = Directory.GetCurrentDirectory();
DotEnv.Load(rootDir);

var builder = WebApplication.CreateBuilder(args);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
