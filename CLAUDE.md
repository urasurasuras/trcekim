# CLAUDE.md

TRÇekim: a website for a school-event photography/videography contractor. Parents open
their child's event gallery with an access code and order prints. The photographer
manages schools, events, galleries, uploads and orders.

This repo revives the 2012–2016 ASP.NET WebForms site (in `legacy/`) as a modern app.

- Feature list and milestones: `docs/features.md`. This is the source of truth for scope;
  update it when scope changes.
- How the old system worked, its schema and its flaws: `docs/legacy-analysis.md`.

## Repo layout

- `legacy/httpdocs/`: the old site, kept as **read-only reference**. Never edit or deploy it.
  Mine it for content (texts, contact info, prices, school list) and assets (logo, school
  logos, slider images).
- `private/` (git-ignored): FTP/DB credential notes, a duplicate old copy, and the unredacted
  `conn.cs`. Never commit it, and never copy anything from it into tracked files.
- `src/`, `tests/`: the new app (not scaffolded yet; see Target stack).
- `docs/`: analysis and feature list.

## Target stack (decided defaults, change only with the user)

- **.NET 10 (LTS) Blazor Web App**. Pages render as **static SSR** by default (public site
  and parent galleries: fast, SEO-friendly, cheap to host). Use `@rendermode InteractiveServer`
  only on admin pages and small interactive islands (upload progress, cart). No WebAssembly.
- **EF Core + SQLite** (one file, easy to back up). Keep the provider swappable for PostgreSQL;
  avoid SQLite-only SQL.
- **Photos on disk** behind an `IPhotoStore` abstraction (local volume now, S3-compatible
  later). Never put photo files in `wwwroot`. Serve them through an endpoint that checks
  gallery access.
- Image processing: **SkiaSharp** (MIT). Avoid ImageSharp (its license has a revenue
  threshold) unless the user agrees.
- Auth: ASP.NET Core Identity for admins only. Parents use an access code that unlocks a
  gallery for the session (cookie). Optional parent accounts later.
- Hosting: one Linux **Docker** image (`mcr.microsoft.com/dotnet/aspnet`), with volumes for
  the DB and the photos, behind a reverse proxy. It runs on any VPS or on the owner's
  home-lab (OMV / Nginx Proxy Manager). There is no IIS or Windows dependency.

Planned solution layout (create it when scaffolding):

```
src/TrCekim.Web/      Blazor Web App (UI, endpoints, DI wiring)
src/TrCekim.Core/     domain entities, services, interfaces (no ASP.NET deps)
src/TrCekim.Data/     EF Core DbContext, migrations, IPhotoStore impl
tests/TrCekim.Tests/  xUnit
```

## Domain vocabulary (legacy Turkish → new code)

`okul` → School · `etkinlik` → Event · `sınıf` → class/Group · gallery = one event for
one group · `referansKod`/`randomSifre` → AccessCode · `resim` → Photo · photo number
(`orjinalResimPath`, camera number like `017988`) → `Photo.Number`; parents quote it,
so keep it visible · `Buyuk`/`Kucuk` → web/thumb renditions · `sepet` → Cart ·
`veli` → Parent · `öğrenci` → Student · `yakınlık` → relation · `kargo adresi` → shipping address.

## Conventions

- Code, identifiers and comments are in English. UI text is in **Turkish**, through
  `IStringLocalizer` resources, not hard-coded strings, so English can be added later.
- Turkish casing: use `CultureInfo("tr-TR")` for display, and `ToUpperInvariant` /
  ordinal comparisons for keys and codes (the `i/İ` trap).
- Prices are `decimal` in TRY. Never use floating point for money.
- Access codes come from a crypto RNG, use an unambiguous alphabet, are at least 8 chars,
  are not sequential, are stored hashed or unique-indexed, and lookups are rate-limited.

## Security and privacy rules (children's photos, KVKK)

- Galleries are private: no directory listing, `noindex`, and no guessable file URLs.
  Every photo request checks access.
- Parameterized queries only (EF Core), and passwords only through Identity.
- Secrets go in user-secrets or environment variables, never in `appsettings*.json` in git.
- Contact form: fixed recipient from config, antiforgery token, rate limit, and a honeypot
  or captcha.
- Design for deletion: galleries and their files must be purgeable (retention policy).

## Commands

Not scaffolded yet, and this container has **no `dotnet` SDK or Docker**. Once the solution
exists, the standard commands are:

```
dotnet build
dotnet test
dotnet run --project src/TrCekim.Web
dotnet ef migrations add <Name> -p src/TrCekim.Data -s src/TrCekim.Web
docker build -t trcekim .
```

Update this section when the real commands exist.
