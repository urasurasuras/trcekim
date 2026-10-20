# CLAUDE.md

TRÇekim: a website for a school-event photography/videography contractor. Parents open
their child's event gallery with an access code and order prints. The photographer
manages schools, events, galleries, uploads and orders.

This repo revives the 2012–2016 ASP.NET WebForms site as a modern app. The old site is no
longer in the tree; it is kept in git history under the tag `legacy-webforms`.

- Feature list and milestones: `docs/features.md`. This is the source of truth for scope;
  update it when scope changes.
- How the old system worked, its schema and its flaws: `docs/legacy-analysis.md`.

## Repo layout

- Old site: tag `legacy-webforms`, folder `legacy/httpdocs/`. It is **read-only reference**:
  never restore, edit or deploy it. Read it with `git show legacy-webforms:legacy/httpdocs/<file>`,
  or check it out with `git worktree add ../trcekim-legacy legacy-webforms`. Mine it for
  content (texts, contact info, prices, school list) and assets (logo, school logos, slider images).
- `private/` (git-ignored): FTP/DB credential notes, a duplicate old copy, and the unredacted
  `conn.cs`. Never commit it, and never copy anything from it into tracked files.
- `src/TrCekim.Core/`: entities, access-code rules, interfaces (`IPhotoStore`, `IImageProcessor`). No ASP.NET deps.
- `src/TrCekim.Data/`: `AppDbContext` (EF Core + Identity), migrations, `FileSystemPhotoStore`, `SkiaImageProcessor`.
- `src/TrCekim.Web/`: the Blazor Web App.
  - `Components/Pages/*.razor`: public pages, static SSR.
  - `Components/Pages/Admin/*.razor`: admin pages, `[Authorize]` via `_Imports`, mostly InteractiveServer.
  - `Services/`: gallery access cookie, cart and orders, photo and admin endpoints, mail, startup.
- `tests/TrCekim.Tests/`: xUnit unit tests plus `WebApplicationFactory` HTTP tests (`WebTests.cs`).
- `CNAME`, `index.html`, `media/`: the GitHub Pages "under construction" page that serves
  www.trcekim.com from `main`. Leave them alone until hosting is switched.
- `deploy/omv/`: the owner's OMV server. `trcekim.yml` is one environment (prod = `main` on
  port 8090, dev = `dev` on port 8091). `trcekim-infra/` runs the deployer, which rebuilds an
  environment when its branch moves, and Mailpit, which catches all outgoing mail. All of it
  is managed as stacks in the OMV Compose UI. See its README.

## Branches

`main` is production (trcekim.uras.app) and `dev` is the dev site (dev-trcekim.uras.app).
Both redeploy automatically on OMV when they move. Work on a feature branch, open a PR into
`dev`, then a PR from `dev` into `main`. Never push to `main` directly.
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

How the main pieces work:

- **Gallery access**: `GalleryAccess` keeps the list of unlocked gallery IDs in a Data
  Protection cookie. `/foto/{photoId}/{thumb|web|original}` checks it on every request.
  Originals are admin-only.
- **SSR forms**: public forms are `EditForm` with a `FormName` plus `[SupplyParameterFromForm]`.
  Initialise the model with `Input ??= new()` (not a property initializer). `CodeForm.razor` is a
  plain HTML form that posts `_handler=code` to `/galeri`, so it works on any page.
- **Runtime data**: everything lives under `DataDirectory` (default `src/TrCekim.Web/data/`;
  `/data` in Docker): `trcekim.db`, `photos/`, `keys/`. Migrations run at startup. The first
  admin is created from `Admin:Email` / `Admin:Password` when no user exists.
- **SQLite quirk**: `DateTimeOffset` is stored as a long (converter in `AppDbContext`), so do
  date comparisons and ordering in memory after `ToListAsync()`, not in SQL. Decimal sums
  are done in memory too.

## Domain vocabulary (legacy Turkish → new code)

`okul` → School · `etkinlik` → Event · `sınıf` → class/Group · gallery = one event for
one group · `referansKod`/`randomSifre` → AccessCode · `resim` → Photo · photo number
(`orjinalResimPath`, camera number like `017988`) → `Photo.Number`; parents quote it,
so keep it visible · `Buyuk`/`Kucuk` → web/thumb renditions · `sepet` → Cart ·
`veli` → Parent · `öğrenci` → Student · `yakınlık` → relation · `kargo adresi` → shipping address.

## Conventions

- Code, identifiers and comments are in English. UI text is in **Turkish**, written directly
  in the Razor markup. Move it to resources only if a second language is actually needed.
- Blazor HTML-escapes non-ASCII characters by default; `Program.cs` sets `WebEncoderOptions`
  to `UnicodeRanges.All`. Keep that setting, or Turkish text renders as `&#xC7;` entities.
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

```
dotnet build
dotnet test                                    # unit + HTTP tests, ~10 s
dotnet run --project src/TrCekim.Web           # dev; set Admin__Email / Admin__Password once
dotnet tool restore                            # once, for dotnet-ef
dotnet ef migrations add <Name> -p src/TrCekim.Data -s src/TrCekim.Web -o Migrations
docker compose up -d --build                   # production-like
```

In the code-server container, the .NET SDK is a user-space install: `/config/.dotnet`, with
`/config/.local/bin/dotnet` as a wrapper that sets `DOTNET_ROOT` and the ICU library path.
There is no Docker here, so the Dockerfile is only verified elsewhere.

After changing UI behaviour, add or extend a test in `WebTests.cs`. The interactive admin
pages (upload, inline editing) are not covered by HTTP tests, so check them in a browser.
