# Deploy on the OMV server

Two environments run side by side on the OMV VM (192.168.0.2). Each one redeploys
itself when its branch moves, which normally happens when a PR is merged into it.
Everything is visible and editable in the OMV web UI under Services → Compose.

| OMV stack | Branch | Container | Port | Data (`appdata/…`) | Public URL |
|---|---|---|---|---|---|
| `trcekim` | `main` | `trcekim` | 8090 | `trcekim` | https://trcekim.uras.app |
| `trcekim-dev` | `dev` | `trcekim-dev` | 8091 | `trcekim-dev` | https://dev-trcekim.uras.app |
| `trcekim-infra` | – | `trcekim-deployer`, `trcekim-mailpit` | 8025 (Mailpit) | `trcekim-deployer`, `trcekim-mailpit` | LAN only |

- `trcekim-deployer` polls GitHub every 60 s. When `main` or `dev` points to a new commit,
  it checks the commit out, builds the image with that commit's `deploy/omv/trcekim.yml`
  and the env of the OMV stack of the same name, and restarts the container. If the build
  or start fails, the old container keeps running. See `docker logs trcekim-deployer`.
- `trcekim-mailpit` is a local SMTP server that catches every e-mail from both sites.
  Nothing is sent out. Read the mail at http://192.168.0.2:8025.

`appdata` is `/srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/appdata`.

## What lives where

| What | Source of truth | Edit it in |
|---|---|---|
| Site service (image build, ports, volumes) | `deploy/omv/trcekim.yml` on the branch | a PR |
| Site settings (port, data folder, admin, SMTP) | env of OMV stacks `trcekim` / `trcekim-dev` | OMV UI |
| Site stack body (an `include:` of the deployer's checkout) | `omv-site.yml` | OMV UI (rarely) |
| Deployer and Mailpit | `trcekim-infra/` | OMV UI: compose file and Dockerfile entry `trcekim-infra` |

Changes to the env of a site take effect on the next deploy, or right away with
"Up" on the stack in OMV. Changes to `trcekim-infra/` in git are not picked up
automatically: paste them into the OMV entries and press "Up" (with build) on
`trcekim-infra`.

## First setup

On the OMV host shell:

```bash
APPDATA=/srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/appdata

# Data folders, owned like the other appdata (PUID 1000 / PGID 100)
mkdir -p $APPDATA/trcekim $APPDATA/trcekim-dev $APPDATA/trcekim-mailpit $APPDATA/trcekim-deployer/ssh
chown 1000:100 $APPDATA/trcekim $APPDATA/trcekim-dev $APPDATA/trcekim-mailpit

# Read-only deploy key for the private GitHub repo
ssh-keygen -t ed25519 -N '' -C trcekim-deployer -f $APPDATA/trcekim-deployer/ssh/id_ed25519
echo 'github.com ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl' \
  > $APPDATA/trcekim-deployer/ssh/known_hosts
cat $APPDATA/trcekim-deployer/ssh/id_ed25519.pub
# GitHub → repo → Settings → Deploy keys → Add deploy key (leave "Allow write access" off)

# Network shared by Mailpit and the sites
docker network create trcekim-mail
```

Then in the OMV web UI (Services → Compose):

1. **Dockerfiles → Add**, name `trcekim-infra`: paste `trcekim-infra/Dockerfile` as the
   Dockerfile, `trcekim-infra/deploy.sh` as the script, and `deploy.sh` as the script filename.
2. **Files → Add**, name `trcekim-infra`: paste `trcekim-infra/trcekim-infra.yml` as the
   file and `trcekim-infra/trcekim-infra.env.example` as the environment. Save, then "Up".
3. **Files → Add**, name `trcekim-dev`: paste `omv-site.yml` as the file and
   `trcekim-dev.env.example` as the environment, with `ADMIN_EMAIL` and, for the first start
   only, `ADMIN_PASSWORD`. Do not press "Up": the deployer starts it within a minute.
4. **Files → Add**, name `trcekim`: the same with `trcekim.env.example`.

Log in at `http://192.168.0.2:8090/admin/giris` (prod) and `:8091` (dev). Then remove
`ADMIN_PASSWORD` from the stack's environment. It is only used when no admin exists yet.

## Everyday flow

1. Work on a feature branch and open a PR into `dev`. Merging it redeploys dev within a minute.
2. Check the change on https://dev-trcekim.uras.app.
3. Open a PR from `dev` into `main`. Merging it redeploys prod.

To roll back, revert the PR on GitHub, which moves the branch again. To redeploy the same
commit, for example after a failed build, delete the state files and wait a minute:

```bash
rm $APPDATA/trcekim-deployer/trcekim.failed $APPDATA/trcekim-deployer/trcekim.deployed
```

## Copy prod data to dev

Stop both containers first: SQLite must not be copied while it is being written.

```bash
docker stop trcekim trcekim-dev
rsync -a --delete $APPDATA/trcekim/ $APPDATA/trcekim-dev/
docker start trcekim trcekim-dev
```

## Nginx Proxy Manager (192.168.0.3)

Add one proxy host per environment: `trcekim.uras.app` forwards to `http://192.168.0.2:8090`,
and `dev-trcekim.uras.app` forwards to `http://192.168.0.2:8091`. Enable "Websockets support"
(the admin pages use a live connection), "Block common exploits" and a Let's Encrypt
certificate with "Force SSL". The app already trusts `X-Forwarded-*` headers
(`TrustForwardedHeaders=true` in the image). In Cloudflare, add both names as DNS records
pointing at the public IP.

## Backup

Back up `appdata/trcekim` as a whole: `trcekim.db`, `photos/` and `keys/`. Without `keys/`,
admin logins and parents' unlocked-gallery cookies are reset, but no data is lost.

## Troubleshooting

- `fetching main failed` with `Permission denied (publickey)`: the deploy key is missing
  from GitHub.
- `main has no deploy/omv/trcekim.yml, skipped`: the branch does not contain the app yet.
  It deploys after the next merge. Until then, the OMV stack of that environment shows an
  error, because its `include:` path does not exist.
- `missing /config/trcekim/trcekim.env`: the OMV stack of that name does not exist yet.
- `SQLite Error 14: unable to open database file`: the data folder is not owned by 1000:100.
- `network trcekim-mail declared as external, but could not be found`: run
  `docker network create trcekim-mail`.
- The admin page shows "Sunucuya yeniden bağlanılıyor…" (reconnecting) behind the proxy:
  enable Websockets support on the NPM proxy host.
