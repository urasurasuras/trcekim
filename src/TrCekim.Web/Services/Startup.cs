using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrCekim.Data;

namespace TrCekim.Web.Services;

public static class Startup
{
    /// <summary>
    /// Applies migrations and, when no admin exists yet, creates one from the
    /// <c>Admin:Email</c> / <c>Admin:Password</c> settings (env vars or user-secrets).
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration config, ILogger log)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        if (await users.Users.AnyAsync())
        {
            return;
        }

        var email = config["Admin:Email"];
        var password = config["Admin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            log.LogWarning("No admin user exists. Set Admin__Email and Admin__Password and restart to create one.");
            return;
        }

        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await users.CreateAsync(user, password);
        if (result.Succeeded)
        {
            log.LogInformation("Created admin user {Email}", email);
        }
        else
        {
            log.LogError("Could not create admin user: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}
