using Microsoft.EntityFrameworkCore;
using TrCekim.Core;
using TrCekim.Data;

namespace TrCekim.Web.Services;

public sealed record CartItem(
    int LineId, Guid PhotoId, Guid GalleryId, string PhotoNumber, string GalleryName, string SchoolName,
    int ProductId, string ProductName, decimal UnitPrice, int Quantity)
{
    public decimal LineTotal => UnitPrice * Quantity;
}

public sealed record CheckoutForm(
    string StudentName, string ParentName, string? Relation, string Phone, string? Email,
    DeliveryMethod Delivery, string? ShippingAddress, string? Note);

/// <summary>Anonymous cart keyed by a random id in a cookie, and order placement.</summary>
public sealed class CartService(IDbContextFactory<AppDbContext> dbFactory, GalleryAccess access, TimeProvider clock)
{
    public const string CookieName = "trc_cart";
    public const int MaxQuantity = 99;

    public async Task<IReadOnlyList<CartItem>> GetItemsAsync(HttpContext http)
    {
        if (ReadCartId(http) is not { } cartId)
        {
            return [];
        }

        await using var db = await dbFactory.CreateDbContextAsync();
        var items = await db.CartLines
            .Where(l => l.CartId == cartId)
            .Select(l => new CartItem(
                l.Id, l.PhotoId, l.Photo!.GalleryId, l.Photo.Number,
                l.Photo.Gallery!.GroupName == null
                    ? l.Photo.Gallery.Title + " (" + l.Photo.Gallery.Season + ")"
                    : l.Photo.Gallery.GroupName + " – " + l.Photo.Gallery.Title + " (" + l.Photo.Gallery.Season + ")",
                l.Photo.Gallery.School!.Name,
                l.ProductId, l.Product!.Name, l.Product.Price, l.Quantity))
            .ToListAsync();

        // Drop lines whose gallery the visitor can no longer open (expired cookie, unpublished).
        return items.Where(i => access.CanView(http, i.GalleryId)).OrderBy(i => i.GalleryName).ThenBy(i => i.PhotoNumber).ToList();
    }

    public async Task AddAsync(HttpContext http, Guid photoId, int productId, int quantity)
    {
        quantity = Math.Clamp(quantity, 1, MaxQuantity);

        await using var db = await dbFactory.CreateDbContextAsync();
        var photo = await db.Photos.Include(p => p.Gallery).FirstOrDefaultAsync(p => p.Id == photoId)
            ?? throw new InvalidOperationException("Fotoğraf bulunamadı.");
        if (!access.CanView(http, photo.GalleryId) || !photo.Gallery!.IsOpen(clock.GetUtcNow()))
        {
            throw new InvalidOperationException("Bu galeriye erişiminiz yok.");
        }
        if (!await db.Products.AnyAsync(p => p.Id == productId && p.IsActive))
        {
            throw new InvalidOperationException("Ürün bulunamadı.");
        }

        var cartId = ReadCartId(http);
        Cart? cart = cartId is null ? null : await db.Carts.FindAsync(cartId.Value);
        if (cart is null)
        {
            cart = new Cart();
            db.Carts.Add(cart);
            WriteCartId(http, cart.Id);
        }
        cart.UpdatedAt = clock.GetUtcNow();

        var line = await db.CartLines.FirstOrDefaultAsync(l =>
            l.CartId == cart.Id && l.PhotoId == photoId && l.ProductId == productId);
        if (line is null)
        {
            db.CartLines.Add(new CartLine { CartId = cart.Id, PhotoId = photoId, ProductId = productId, Quantity = quantity });
        }
        else
        {
            line.Quantity = Math.Min(MaxQuantity, line.Quantity + quantity);
        }
        await db.SaveChangesAsync();
    }

    /// <summary>Sets a line's quantity; 0 removes it.</summary>
    public async Task UpdateAsync(HttpContext http, int lineId, int quantity)
    {
        if (ReadCartId(http) is not { } cartId)
        {
            return;
        }

        await using var db = await dbFactory.CreateDbContextAsync();
        var line = await db.CartLines.FirstOrDefaultAsync(l => l.Id == lineId && l.CartId == cartId);
        if (line is null)
        {
            return;
        }
        if (quantity <= 0)
        {
            db.CartLines.Remove(line);
        }
        else
        {
            line.Quantity = Math.Min(MaxQuantity, quantity);
        }
        await db.SaveChangesAsync();
    }

    public async Task<Order> PlaceOrderAsync(HttpContext http, CheckoutForm form)
    {
        var items = await GetItemsAsync(http);
        if (items.Count == 0)
        {
            throw new InvalidOperationException("Sepetiniz boş.");
        }

        var order = new Order
        {
            Reference = OrderReferences.Generate(),
            CreatedAt = clock.GetUtcNow(),
            StudentName = form.StudentName.Trim(),
            ParentName = form.ParentName.Trim(),
            Relation = Clean(form.Relation),
            Phone = form.Phone.Trim(),
            Email = Clean(form.Email),
            Delivery = form.Delivery,
            ShippingAddress = form.Delivery == DeliveryMethod.Shipping ? Clean(form.ShippingAddress) : null,
            Note = Clean(form.Note),
            Lines = items.Select(i => new OrderLine
            {
                PhotoId = i.PhotoId,
                GalleryName = i.GalleryName,
                SchoolName = i.SchoolName,
                PhotoNumber = i.PhotoNumber,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
            }).ToList(),
        };
        order.Total = order.Lines.Sum(l => l.LineTotal);

        await using var db = await dbFactory.CreateDbContextAsync();
        db.Orders.Add(order);
        if (ReadCartId(http) is { } cartId)
        {
            await db.Carts.Where(c => c.Id == cartId).ExecuteDeleteAsync();
        }
        await db.SaveChangesAsync();
        http.Response.Cookies.Delete(CookieName);
        return order;
    }

    private static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static Guid? ReadCartId(HttpContext http) =>
        http.Request.Cookies.TryGetValue(CookieName, out var raw) && Guid.TryParse(raw, out var id) ? id : null;

    private static void WriteCartId(HttpContext http, Guid id) =>
        http.Response.Cookies.Append(CookieName, id.ToString("N"), new CookieOptions
        {
            HttpOnly = true,
            Secure = http.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            MaxAge = TimeSpan.FromDays(14),
        });
}
