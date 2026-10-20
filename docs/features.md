# Feature list

Status: **L** = live in the legacy site, **P** = partial / unfinished in legacy,
**N** = new (implied by the business or needed to replace a manual step).
Priority: **M1** = rebuild parity (go-live), **M2** = ordering, **M3** = nice to have.

## Implementation status (Blazor rewrite)

- **Done:** 1, 2, 3, 4, 7, 8 (draft text; needs legal review), 9, 11, 12, 13, 14, 19, 20, 21,
  22 v1 (offline payment instructions), 23 (confirmation page, status tracking, admin
  e-mail; no customer e-mail yet), 24 (login and lockout; no 2FA yet), 25, 26, 27, 28, 29,
  30, 35 (logo).
- **Not started:** 5, 6, 10, 15 (video), 16 (watermark), 17, 18, 22 v2 (online payment),
  31, 32 (retention purge job), 33, 34. Also 2FA for admins and a customer confirmation e-mail.

## Public site

| # | Feature | Legacy | Prio |
|---|---|---|---|
| 1 | Home page: hero/slider of showcase photos, short intro, call-to-action "Galerinize giriş" | L | M1 |
| 2 | About us (Hakkımızda / vizyon), company info (Dizi Production subsidiary) | P (placeholder) | M1 |
| 3 | Services and products: event photography, videography, print sizes, posters (38×21), photo collages/tables, wallpaper prints | P (announcements on `okullar.aspx`) | M1 |
| 4 | References: schools served, with logos (`images/resimler/okul_logolari`) | P (commented out) | M2 |
| 5 | Announcements / news | P | M3 |
| 6 | Shoot schedule (çekim programı) by month | P | M3 |
| 7 | Contact page: address, phones, e-mail, map, server-side contact form (spam protected, rate limited, fixed recipient) | L (insecure PHP) | M1 |
| 8 | Privacy policy / KVKK notice, cookie notice | P (dead link) | M1 |
| 9 | Turkish UI (localizable; English later if needed) | L | M1 |
| 10 | Privacy-friendly analytics (replace dead GA UA) | L | M3 |

## Client galleries (parents)

| # | Feature | Legacy | Prio |
|---|---|---|---|
| 11 | Enter gallery by access code (Referans No), then open that event's gallery | L | M1 |
| 12 | Strong, random, non-sequential codes; rate-limited attempts; optional expiry date | N (legacy codes guessable) | M1 |
| 13 | Gallery page: title (school, season, class, event), responsive thumbnail grid, lightbox with next/prev, visible photo number | L | M1 |
| 14 | Images served only to holders of a valid code (no public guessable URLs), `noindex` | N | M1 |
| 15 | Video items in galleries (event films, streamed or downloadable) | P (empty `video/`, jPlayer) | M2 |
| 16 | Optional: watermark on web-size images | N | M2 |
| 17 | Optional: digital download of purchased/originals | N | M3 |
| 18 | Optional parent accounts (see all their galleries in one place); access code stays the primary path | P (`yeniKullanici`/`kullaniciGirisi`) | M3 |

## Ordering

| # | Feature | Legacy | Prio |
|---|---|---|---|
| 19 | Add photo to cart with size and quantity: 15×23, 20×30, 50×70 (legacy prices 5 / 15 / 70 TL), live subtotal | P (UI only, disabled) | M2 |
| 20 | Admin-managed product/price list (print sizes, posters, collages, video DVD/USB/download) | N | M2 |
| 21 | Checkout: student name + class, parent name, relation, phone, e-mail, delivery (via school or shipping address) | P (`kisiler` fields) | M2 |
| 22 | Payment v1: offline (bank transfer / cash on delivery via school); v2: online (iyzico / PayTR) | P (phone orders) | M2 / M3 |
| 23 | Order confirmation e-mail; order status (received → printing → delivered) | N | M2 |

## Admin (photographer)

| # | Feature | Legacy | Prio |
|---|---|---|---|
| 24 | Secure admin login (ASP.NET Core Identity, 2FA) | N | M1 |
| 25 | Manage schools, seasons, events, galleries (class / group) | P (file naming + XML) | M1 |
| 26 | Bulk upload photos to a gallery (multi-file / zip); keep camera photo numbers from file names | P (FTP + Excel) | M1 |
| 27 | Automatic web-size + thumbnail generation (replaces Buyuk/Kucuk exports and the Excel sheet) | P (manual) | M1 |
| 28 | Generate access codes per gallery; printable code slips (per student / per class) | P (manual) | M1 |
| 29 | Publish / unpublish / expire galleries; delete gallery + files | N | M1 |
| 30 | Orders dashboard; export per-school print list (CSV) for the print lab | N | M2 |
| 31 | Gallery view statistics | N | M3 |
| 32 | Data retention: auto-purge galleries after N months (KVKK, children's data) | N | M2 |

## Migration

| # | Feature | Legacy | Prio |
|---|---|---|---|
| 33 | Import `referanslar.xml` and the 128 gallery pages (title, code, photo list) into the DB | n/a | M1 (only if the old galleries are still needed) |
| 34 | Import photo files, if the originals turn up (they are not in this archive) | n/a | M3 |
| 35 | Reuse surviving assets: logo, school logos, home-page slider sets | n/a | M1 |

## Explicitly dropped

The template demo pages, the PHP search, `uyeler.aspx`, the `.asmx` stubs, TripleDES URL
"encryption", IE6–8 shims, Flash, and the Highslide / Camera / Kwicks plugins.
