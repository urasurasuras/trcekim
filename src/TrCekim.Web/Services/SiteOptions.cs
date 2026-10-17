namespace TrCekim.Web.Services;

/// <summary>Public business details shown on the site ("Site" config section).</summary>
public sealed class SiteOptions
{
    public string Name { get; set; } = "TRÇekim";
    public string Tagline { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Mobile { get; set; } = "";
    public string Email { get; set; } = "";

    /// <summary>Payment instructions shown after an order (e.g. IBAN). Plain text, line breaks kept.</summary>
    public string PaymentInstructions { get; set; } = "";
}

/// <summary>Outgoing mail ("Smtp" section). Leave Host empty to disable e-mail.</summary>
public sealed class SmtpOptions
{
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? From { get; set; }

    /// <summary>Where contact-form and order notifications go.</summary>
    public string? NotifyTo { get; set; }
}
