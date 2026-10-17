using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using TrCekim.Core;

namespace TrCekim.Web.Services;

/// <summary>
/// E-mail notifications to the business. Everything is also stored in the DB and
/// visible in the admin panel, so mail is best-effort: failures are logged, not thrown.
/// </summary>
public sealed class Notifier(IOptions<SmtpOptions> options, ILogger<Notifier> log)
{
    private static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

    public Task ContactReceivedAsync(ContactMessage m) => SendAsync(
        $"İletişim formu: {m.Name}",
        $"Ad: {m.Name}\nE-posta: {m.Email}\nTelefon: {m.Phone}\n\n{m.Message}",
        replyTo: m.Email);

    public Task OrderPlacedAsync(Order o)
    {
        var sb = new StringBuilder()
            .AppendLine($"Sipariş: {o.Reference}")
            .AppendLine($"Öğrenci: {o.StudentName}")
            .AppendLine($"Veli: {o.ParentName} ({o.Relation}) {o.Phone} {o.Email}")
            .AppendLine($"Teslimat: {(o.Delivery == DeliveryMethod.Shipping ? o.ShippingAddress : "Okul aracılığıyla")}")
            .AppendLine();
        foreach (var l in o.Lines)
        {
            sb.AppendLine($"{l.SchoolName} / {l.GalleryName} / Foto {l.PhotoNumber} – {l.ProductName} × {l.Quantity} = {l.LineTotal.ToString("C", Tr)}");
        }
        sb.AppendLine().AppendLine($"Toplam: {o.Total.ToString("C", Tr)}");
        if (!string.IsNullOrEmpty(o.Note))
        {
            sb.AppendLine().AppendLine($"Not: {o.Note}");
        }
        return SendAsync($"Yeni sipariş {o.Reference}", sb.ToString(), replyTo: o.Email);
    }

    private async Task SendAsync(string subject, string body, string? replyTo)
    {
        var o = options.Value;
        if (string.IsNullOrWhiteSpace(o.Host) || string.IsNullOrWhiteSpace(o.NotifyTo) || string.IsNullOrWhiteSpace(o.From))
        {
            log.LogInformation("SMTP not configured; skipped e-mail \"{Subject}\"", subject);
            return;
        }

        try
        {
            using var msg = new MailMessage(o.From, o.NotifyTo, subject, body);
            if (!string.IsNullOrWhiteSpace(replyTo) && MailAddress.TryCreate(replyTo, out var reply))
            {
                msg.ReplyToList.Add(reply);
            }
            using var client = new SmtpClient(o.Host, o.Port) { EnableSsl = o.EnableSsl };
            if (!string.IsNullOrEmpty(o.User))
            {
                client.Credentials = new NetworkCredential(o.User, o.Password);
            }
            await client.SendMailAsync(msg);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Sending e-mail \"{Subject}\" failed", subject);
        }
    }
}
