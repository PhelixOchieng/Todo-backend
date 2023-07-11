using Microsoft.EntityFrameworkCore;

using Todo.Models;
using Users.Models;

namespace Application.Core.Context;

public class ApplicationDbContext : DbContext {
  public DbSet<TodoItem> TodoItems { get; set; } = null!;
  public DbSet<User> Users { get; set; } = null!;

	public ApplicationDbContext(DbContextOptions options) : base(options) {}

  protected override void
  OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    string dbURL = parseDBURLFromEnv();
    optionsBuilder.UseNpgsql(dbURL).UseSnakeCaseNamingConvention();
  }

  private string parseDBURLFromEnv() {
    if (Environment.GetEnvironmentVariable("DATABASE_URL") is not string rawURL)
      throw new InvalidOperationException(
          "DATABASE_URL missing in environment");

    if (rawURL.Split(':', StringSplitOptions.RemoveEmptyEntries)
            is not string[] splitURL ||
        splitURL.Length != 4 ||
        splitURL[2].Split('@') is not string[] passwordHostSplit ||
        splitURL[3].Split('/') is not string[] portDBSplit)
      throw new ArgumentException(
          $"DATABASE_URL is malformed. Received \"{rawURL}\" but expected \"postgres://<username>:<password>@<host>:<port>/<database>\" ");

    string username = splitURL[1].Replace("/", "");
    string password = passwordHostSplit[0];
    string host = passwordHostSplit[1];
    string port = portDBSplit[0];
    string db = portDBSplit[1];

    return $"Host={host};Database={db};Username={username};Password={password};Port={port}";
  }
}
