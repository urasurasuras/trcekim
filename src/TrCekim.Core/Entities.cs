namespace TrCekim.Core;

/// <summary>A customer school (legacy: okul / birim).</summary>
public class School
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? City { get; set; }

    /// <summary>Shown on the public references list (opt-in, schools are customers).</summary>
    public bool ShowAsReference { get; set; }

    public List<Gallery> Galleries { get; set; } = [];
}

/// <summary>
/// One event for one group: e.g. "2015-2016 / 4-A / Sene sonu gösterisi".
/// Parents unlock it with <see cref="AccessCode"/> (legacy: referansKod / randomSifre).
/// </summary>
public class Gallery
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int SchoolId { get; set; }
    public School? School { get; set; }

    /// <summary>School year, e.g. "2025-2026".</summary>
    public required string Season { get; set; }

    /// <summary>Event name, e.g. "Mezuniyet", "Okuma bayramı".</summary>
    public required string Title { get; set; }

    /// <summary>Class or group, e.g. "4-A". Optional for whole-school events.</summary>
    public string? GroupName { get; set; }

    public DateOnly? EventDate { get; set; }

    /// <summary>Normalized code (see <see cref="AccessCodes.Normalize"/>); unique.</summary>
    public required string AccessCode { get; set; }

    public bool IsPublished { get; set; }

    /// <summary>After this instant the code no longer opens the gallery.</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<Photo> Photos { get; set; } = [];

    public bool IsOpen(DateTimeOffset now) => IsPublished && (ExpiresAt is null || ExpiresAt > now);

    public string DisplayName =>
        GroupName is null ? $"{Title} ({Season})" : $"{GroupName} – {Title} ({Season})";
}

public class Photo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GalleryId { get; set; }
    public Gallery? Gallery { get; set; }

    /// <summary>
    /// Number parents quote when ordering; taken from the camera file name (e.g. "017988").
    /// </summary>
    public required string Number { get; set; }

    public string? OriginalFileName { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>Something a parent can order for a photo: a print size, poster, etc.</summary>
public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    /// <summary>Unit price in TRY.</summary>
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

/// <summary>Anonymous cart, identified by a random token kept in a cookie.</summary>
public class Cart
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<CartLine> Lines { get; set; } = [];
}

public class CartLine
{
    public int Id { get; set; }
    public Guid CartId { get; set; }
    public Guid PhotoId { get; set; }
    public Photo? Photo { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
}

public enum OrderStatus
{
    Received = 0,
    Confirmed = 1,
    Printing = 2,
    Delivered = 3,
    Cancelled = 9,
}

public enum DeliveryMethod
{
    ViaSchool = 0,
    Shipping = 1,
}

public class Order
{
    public int Id { get; set; }

    /// <summary>Short reference given to the parent (e.g. for bank transfer description).</summary>
    public required string Reference { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public OrderStatus Status { get; set; }

    public required string StudentName { get; set; }
    public required string ParentName { get; set; }
    public string? Relation { get; set; }
    public required string Phone { get; set; }
    public string? Email { get; set; }
    public DeliveryMethod Delivery { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Note { get; set; }

    public decimal Total { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
}

/// <summary>
/// Snapshot of what was ordered. Copies names and prices so later price or gallery
/// changes do not rewrite history.
/// </summary>
public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Guid? PhotoId { get; set; }
    public required string GalleryName { get; set; }
    public required string SchoolName { get; set; }
    public required string PhotoNumber { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class ContactMessage
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string Message { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsRead { get; set; }
}
