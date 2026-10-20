# Legacy system analysis (trcekim.com, 2012–2016)

Source: `legacy/httpdocs/` at the git tag `legacy-webforms` (removed from the tree later; canonical copy of the production `httpdocs`, taken from
"TRCekim Site kopyasi Can"). A second, older copy plus FTP/DB credential notes live in
the git-ignored `private/` folder.

## What it was

TRÇekim ("TR Çekim", a Dizi Production subsidiary, Seyrantepe / İstanbul) photographed
and filmed school events on contract — year-end shows, graduations (mezuniyet),
reading festivals (okuma bayramı), in-class shoots, cap ceremonies (kep), etc. The site
let parents view the photos of their child's class/event and pick prints to order.

## Hosting / stack

- Windows Plesk shared hosting, IIS (`C:\Inetpub\vhosts\trcekim.com`), ASP.NET WebForms
  on .NET Framework 4.0, `CodeFile` web-site project (no .csproj/.sln), `debug="true"`.
- Front end: a TemplateMonster HTML template (960 grid, jQuery 1.7, superfish menu,
  prettyPhoto + Highslide lightboxes, Camera/Kwicks sliders, jPlayer skins).
- Data: mostly flat files. `referanslar.xml` maps access codes to static HTML pages.
  A MS SQL Server DB `TRCekimDB` (remote, connected as `sa`) backs an unfinished
  login/order flow. A separate MySQL DB (radore phpMyAdmin, `db034707`) is noted in
  `private/` but nothing in the code uses it.
- Deployment: FTP (FileZilla) uploads.
- Analytics: Google Analytics Universal `UA-70919502-1` (UA is shut down).

## Page / file inventory

| File | Role | State |
|---|---|---|
| `Default.aspx` | Home: nav, logo, image slider (`images/resimler/anaSayfaResimleri/SET*`), footer | live |
| `giris.aspx(.cs)` | "Referans No" box. Looks up the code in `referanslar.xml`, redirects to the gallery page | **live: core feature** |
| `*_Ref_<code>.html` (128) + `tokiNilufer.html` | One static gallery per school / year / class / event: title + thumbnail grid + prettyPhoto lightbox | **live: core feature** |
| `SABLON.html` | Gallery page template with `<!--OKUL_ISMI-->` and `<!--BASLANGIC-->` markers | admin tool |
| `ResimYonetimExcel.xlsm` | Excel sheet: photo numbers in column B, then `CONCATENATE` builds the gallery `<div>` rows to paste into the template | admin tool |
| `referanslar.xml` (+ ~13 dated backups) | `<referans><referansKod/><referansDosyasi/></referans>` code→page map, 128 entries | live data |
| `okullar.aspx` | Schools/references list, monthly "çekim programı" (shoot schedule), announcements (poster prints 38×21, photo collages, wallpaper prints) | content commented out |
| `index-5.html` + `bat/MailHandler.php` | Contact page (address, phones, e-mail, map) + contact form posting to PHP mailer | live (PHP on IIS?) |
| `more.html`, `portfolio.html`, `index-*.html`, `typography.html`, `misc.html`, `layouts.html`, `css3.html`, `*slider.html`, `kwicks.html`, `intro.html`, `blank.html`, `404.html`, `under_construction.html`, `social_media.html` | Template demo/placeholder pages ("Hakkımızda", "Vizyon", "Haberler", "Okul Aile Birliği", "Destekçilerimiz") | placeholders |
| `kullaniciGirisi.aspx` | Parent login (username + password) against `kisiler`, then redirect to `okul.aspx?x=<TripleDES(code)>` | unfinished |
| `yeniKullanici.aspx` | Parent registration form | unfinished |
| `okul.aspx` | DB-driven gallery with "SEPETE EKLE" (add to cart) dialog: 15×23 = 5 TL, 20×30 = 15 TL, 50×70 = 70 TL, quantity + subtotal | disabled: alert tells parents to note the photo number and phone in |
| `WebService2.asmx` `sonucGetir2` | Cart endpoint: parses `kisiID~resimID~qty15x23~qty20x30~qty50x70`, persists nothing | stub |
| `WebService.asmx`, `sil.aspx`, `sil1.html`, `deneme_sil1.htm` | Experiments ("sil" = delete) | dead |
| `uyeler.aspx` | Lists **all** registered users (GridView over `kisiler`), no auth | dead but dangerous |
| `search/results.php` | Template site-search | dead |
| `video/` | Empty; jPlayer skins suggest planned video galleries | never built |
| `psd/` | Template design sources (~14 MB, git-ignored) | assets |

## Gallery publishing workflow (manual)

1. Export photos as `images/resimler/<season>/<School>/<refCode>/Buyuk/<photoNo>.JPG`
   (large) and `.../Kucuk/<photoNo>.JPG` (thumbnail). Photo numbers are the camera's
   6-digit sequence (`017988`); parents quote them when ordering.
2. Paste the photo numbers into the Excel sheet, then copy the generated HTML rows.
3. Copy `SABLON.html` to `<SEASON>_<SCHOOL>_<CLASS>_<EVENT>_Ref_<code>.html`, set the
   title between `OKUL_ISMI` markers, paste the rows after `BASLANGIC`.
4. Add a `<referans>` entry to `referanslar.xml` (back up the old file with a date suffix).
5. Upload by FTP. Hand the code to the school/class (printed slips).

Codes are blocks per school/season: 144001–144019 for Fatih Koleji Topkapı 2014-15,
2100xx, 1420xx, and so on. **They are sequential and guessable.**

## Inferred SQL schema (`TRCekimDB`)

- `anaTablo(ID, ustID, turID, resimID, randomSifre, birimAdi, aktif, uploaded)`: a
  self-referencing tree. `turID=1` is a unit/school (`birimAdi` = name), `turID=2` is a
  photo, and `randomSifre` is the access code.
- `resimler(resimID, randomSifre, aciklama, resimPath, orjinalResimPath)`: the photo
  rows. `orjinalResimPath` holds the photo number shown to parents.
- `kisiler(kisiID, kullaniciAdi, sifre, ogrenciAdi, ogrenciSoyadi, yakinlikID, veliAdi,
  veliSoyadi, cep, email, kargoAdresi)`: parent accounts. Passwords are stored in plain text.
- `yakinlik(… , aktif)`: relation to the student (mother, father, …).

## Defects and security problems (do not port)

- SA credentials and the DB server IP were hard-coded in `App_Code/conn.cs`. They are
  redacted in git, and the original is in `private/`. **Treat them as compromised. Rotate
  them, or confirm the server is gone.**
- SQL injection: string-concatenated queries in `kullaniciGirisi.aspx.cs` and `okul.aspx.cs`.
- Plain-text passwords. Unauthenticated `uyeler.aspx` exposes all parent PII.
- `yeniKullanici.aspx.cs` parameter indices are off, so phone, e-mail and address land
  in the wrong columns (`veliSoyadi`, `cep` and `kargoAdresi`).
- Static TripleDES/ECB key `"YOURSECRETKEY"` (sample code left unchanged) protects the gallery URL.
- Sequential short access codes plus static HTML pages. Anyone can enumerate galleries of
  children, and the image URLs are public and guessable.
- `MailHandler.php` is an open mail relay: the recipient (`owner_email`) and `From` header
  come from POST data.
- Static `SqlConnection` shared across requests, swallowed exceptions, `debug="true"` in production.
- Photo originals and thumbnails are **not** in this archive (the `images/resimler/<season>`
  folders are empty). Only home-page sliders, school logos and template images survive.
