using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SPRagAPI.Data;

// Used by `dotnet ef` at design time (migrations). Mirrors Program.cs SQL setup.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("RagDb")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:RagDb is not set. Add it via `dotnet user-secrets set`.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString, sqlOptions =>
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null))
            .Options;

        return new AppDbContext(options);
    }
}
