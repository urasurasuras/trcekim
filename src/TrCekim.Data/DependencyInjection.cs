using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrCekim.Core;

namespace TrCekim.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddTrCekimData(
        this IServiceCollection services, string connectionString, string photoRoot)
    {
        services.AddDbContextFactory<AppDbContext>(o => o.UseSqlite(connectionString));
        services.AddSingleton<IPhotoStore>(new FileSystemPhotoStore(photoRoot));
        services.AddSingleton<IImageProcessor, SkiaImageProcessor>();
        return services;
    }
}
