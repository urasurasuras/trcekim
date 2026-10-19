# Deploy on the OMV server

Runs the site as one container on the OMV VM (192.168.0.97). It sits next to
`claude-code-server` and is reachable at `http://192.168.0.97:8090`.

## First start (OMV host shell)

```bash
# 1. Data folder, owned by the same user as the other appdata (PUID 1000 / PGID 100)
mkdir -p /srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/appdata/trcekim
chown 1000:100 /srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/appdata/trcekim

# 2. Stack directory (same layout as claude-dev)
mkdir -p /docker/docker-compose/trcekim
cd /docker/docker-compose/trcekim
cp /srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/development/trcekim/.claude/worktrees/blazor-rewrite/deploy/omv/trcekim.yml .
cp /srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/development/trcekim/.claude/worktrees/blazor-rewrite/deploy/omv/trcekim.env.example trcekim.env
nano trcekim.env          # set ADMIN_EMAIL and a long ADMIN_PASSWORD

# 3. Build from GitHub and start
docker compose -f trcekim.yml --env-file trcekim.env up -d --build
docker logs -f trcekim    # expect "Created admin user …" and "Now listening on: http://[::]:8080"
```

Open `http://192.168.0.97:8090/admin/giris` and log in. Then remove `ADMIN_PASSWORD`
from `trcekim.env` (it is only used when no admin exists yet) and run `up -d` again.

You can do the same through the OMV web UI: Services → Compose → Files → Add, paste
`trcekim.yml`, and put the env values in the Environment field.

## Update to the latest code

```bash
cd /docker/docker-compose/trcekim
docker compose -f trcekim.yml --env-file trcekim.env build --no-cache
docker compose -f trcekim.yml --env-file trcekim.env up -d
```

The build pulls the branch named in `trcekim.yml` (`#feature/blazor-rewrite`). Change it to
`#main` after the PR is merged.

## Public access (later)

Add a proxy host in Nginx Proxy Manager pointing to `192.168.0.97:8090`, with SSL
(Let's Encrypt) and "Websockets support" **on**. The admin pages use a live WebSocket
connection. The app already trusts `X-Forwarded-*` headers (`TrustForwardedHeaders=true`
in the image). Point the domain only after the GitHub Pages placeholder is turned off.

## Backup

Everything is in `appdata/trcekim`: `trcekim.db`, `photos/` and `keys/`. Back up the
whole folder. Without `keys/`, admin logins and parents' unlocked-gallery cookies are
reset, but no data is lost.

## Troubleshooting

- `SQLite Error 14: unable to open database file`: the appdata folder is not owned by
  1000:100 (step 1).
- Build fails fetching GitHub: the host needs outbound HTTPS, and older Docker needs
  `git` installed for remote build contexts (`apt install git`).
- The admin page shows "Sunucuya yeniden bağlanılıyor…" (reconnecting) behind the proxy:
  enable Websockets support on the NPM proxy host.
