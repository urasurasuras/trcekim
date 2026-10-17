using Microsoft.EntityFrameworkCore;
using TrCekim.Core;

namespace TrCekim.Data;

public static class AppDbContextExtensions
{
    /// <summary>A random access code not used by any gallery yet.</summary>
    public static async Task<string> NewAccessCodeAsync(this AppDbContext db, CancellationToken ct = default)
    {
        while (true)
        {
            var code = AccessCodes.Generate();
            if (!await db.Galleries.AnyAsync(g => g.AccessCode == code, ct))
            {
                return code;
            }
        }
    }
}
