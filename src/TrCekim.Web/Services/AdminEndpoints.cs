using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrCekim.Core;
using TrCekim.Data;

namespace TrCekim.Web.Services;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/admin/cikis", async (SignInManager<IdentityUser> signIn) =>
        {
            await signIn.SignOutAsync();
            return Results.LocalRedirect("~/");
        }).RequireAuthorization();

        // Print list for the lab: one row per order line. Semicolon-separated with a BOM
        // so Turkish Excel opens it with the right columns and characters.
        app.MapGet("/admin/siparisler.csv", async (IDbContextFactory<AppDbContext> dbFactory, OrderStatus? durum) =>
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var query = db.Orders.AsNoTracking().Include(o => o.Lines).AsQueryable();
            if (durum is { } s)
            {
                query = query.Where(o => o.Status == s);
            }
            var orders = (await query.ToListAsync()).OrderBy(o => o.CreatedAt);

            var tr = CultureInfo.GetCultureInfo("tr-TR");
            var sb = new StringBuilder();
            sb.AppendLine("Sipariş;Tarih;Durum;Okul;Galeri;Foto No;Ürün;Adet;Birim Fiyat;Tutar;Öğrenci;Veli;Telefon;E-posta;Teslimat;Adres;Not");
            foreach (var o in orders)
            {
                foreach (var l in o.Lines)
                {
                    sb.AppendJoin(';', new[]
                    {
                        o.Reference, o.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", tr), o.Status.ToString(),
                        l.SchoolName, l.GalleryName, l.PhotoNumber, l.ProductName,
                        l.Quantity.ToString(tr), l.UnitPrice.ToString("0.00", tr), l.LineTotal.ToString("0.00", tr),
                        o.StudentName, o.ParentName, o.Phone, o.Email ?? "",
                        o.Delivery == DeliveryMethod.Shipping ? "Kargo" : "Okul", o.ShippingAddress ?? "", o.Note ?? "",
                    }.Select(Csv)).AppendLine();
                }
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return Results.File(bytes, "text/csv; charset=utf-8", $"siparisler-{DateTime.Now:yyyyMMdd}.csv");
        }).RequireAuthorization();

        return app;
    }

    private static string Csv(string value)
    {
        // Neutralize spreadsheet formula injection, then quote.
        if (value.Length > 0 && "=+-@\t\r".Contains(value[0]))
        {
            value = "'" + value;
        }
        return "\"" + value.Replace("\"", "\"\"").Replace('\n', ' ').Replace("\r", "") + "\"";
    }
}
