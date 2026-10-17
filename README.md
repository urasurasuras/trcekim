# TRÇekim

Website for TRÇekim, a school-event photography and videography service. Parents open
their child's event gallery with an access code and order prints. The photographer
manages schools, galleries, uploads, prices and orders.

It is a .NET 10 Blazor Web App with EF Core + SQLite and SkiaSharp, shipped as one Docker
image. The original 2012–2016 ASP.NET WebForms site is kept in `legacy/` for reference.

## Run locally

Needs the .NET 10 SDK.

```sh
dotnet test
Admin__Email=you@example.com Admin__Password='a-long-password' dotnet run --project src/TrCekim.Web
```

Open the URL it prints, log in at `/admin/giris`, add a school, create a gallery, upload
photos, publish it, and open the gallery from the home page with its code.

## Deploy (Docker)

```sh
cp .env.example .env    # set ADMIN_EMAIL / ADMIN_PASSWORD (+ SMTP_* if you want e-mails)
docker compose up -d --build
```

The app listens on port 8080 over plain HTTP. Put a TLS reverse proxy in front of it
(Nginx Proxy Manager, Caddy, Traefik). All state is in the `/data` volume: SQLite DB,
photos and Data Protection keys. **Back up that volume.** After the first login, remove
`ADMIN_PASSWORD` from `.env`.

Edit the business details shown on the site (address, phones, payment instructions) under
`Site` in `src/TrCekim.Web/appsettings.json`, or override them with `Site__*` environment
variables.

## Docs

- `docs/features.md`: feature list, milestones and implementation status
- `docs/legacy-analysis.md`: how the old site worked and what was wrong with it
- `CLAUDE.md`: architecture, conventions and commands
