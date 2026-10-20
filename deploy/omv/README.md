# Deploy on the OMV server

Two environments run side by side on the OMV VM (192.168.0.2). Each one redeploys
itself when its branch moves, which normally happens when a PR is merged into it.

| Environment | Branch | Compose project / container | Port | Data (`appdata/…`) | Public URL |
|---|---|---|---|---|---|
| prod | `main` | `trcekim` | 8090 | `trcekim` | https://trcekim.uras.app |
| dev | `dev` | `trcekim-dev` | 8091 | `trcekim-dev` | https://dev-trcekim.uras.app |

Shared services (`infra.yml`):

- `trcekim-deployer` polls GitHub every 60 s. When `main` or `dev` points to a new commit,
  it checks the commit out, builds the image with that commit's `deploy/omv/trcekim.yml`
  and restarts the container. If the build or start fails, the old container keeps
  running. See `docker logs trcekim-deployer`.
- `trcekim-mailpit` is a local SMTP server that catches every e-mail from both sites.
  Nothing is sent out. Read the mail at http://192.168.0.2:8025 (LAN only).

Layout on the host:

```
/docker/docker-compose/trcekim/   infra.yml, deployer/, infra.env, trcekim.env, trcekim-dev.env
appdata/trcekim/                  prod: trcekim.db, photos/, keys/
appdata/trcekim-dev/              dev:  same layout, separate copy
appdata/trcekim-deployer/         ssh/ (deploy key), src/ (checkouts), *.deployed / *.failed
appdata/trcekim-mailpit/          mailpit.db
```

`appdata` is `/srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/appdata`.

## First setup (OMV host shell)

```bash
APPDATA=/srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/appdata
REPO=/srv/dev-disk-by-uuid-03d52f78-cc5f-4477-b6b3-1cbf9daac6e0/development/trcekim/.claude/worktrees/blazor-rewrite

# 1. Data folders, owned like the other appdata (PUID 1000 / PGID 100)
mkdir -p $APPDATA/trcekim $APPDATA/trcekim-dev $APPDATA/trcekim-mailpit $APPDATA/trcekim-deployer/ssh
chown 1000:100 $APPDATA/trcekim $APPDATA/trcekim-dev $APPDATA/trcekim-mailpit

# 2. Read-only deploy key for the private GitHub repo
ssh-keygen -t ed25519 -N '' -C trcekim-deployer -f $APPDATA/trcekim-deployer/ssh/id_ed25519
echo 'github.com ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl' \
  > $APPDATA/trcekim-deployer/ssh/known_hosts
cat $APPDATA/trcekim-deployer/ssh/id_ed25519.pub
# GitHub → repo → Settings → Deploy keys → Add deploy key (leave "Allow write access" off)

# 3. Stack directory with the env files
mkdir -p /docker/docker-compose/trcekim && cd /docker/docker-compose/trcekim
cp -r $REPO/deploy/omv/infra.yml $REPO/deploy/omv/deployer .
cp $REPO/deploy/omv/infra.env.example infra.env
cp $REPO/deploy/omv/trcekim.env.example trcekim.env
cp $REPO/deploy/omv/trcekim-dev.env.example trcekim-dev.env
chmod 600 *.env
nano trcekim.env trcekim-dev.env    # ADMIN_EMAIL, and ADMIN_PASSWORD for the first start

# 4. Start Mailpit and the deployer; it builds and starts both sites
docker compose -f infra.yml --env-file infra.env up -d --build
docker logs -f trcekim-deployer     # expect "trcekim-dev: deployed"
```

Then log in at `http://192.168.0.2:8090/admin/giris` (prod) and `:8091` (dev). After that,
remove `ADMIN_PASSWORD` from the env file. It is only used when no admin exists yet.

## Everyday flow

1. Work on a feature branch and open a PR into `dev`. Merging it redeploys dev within a minute.
2. Check the change on https://dev-trcekim.uras.app.
3. Open a PR from `dev` into `main`. Merging it redeploys prod.

To roll back, revert the PR on GitHub, which moves the branch again. To redeploy the same
commit, for example after fixing an env file, delete the state file and wait a minute:

```bash
rm $APPDATA/trcekim-deployer/trcekim.failed $APPDATA/trcekim-deployer/trcekim.deployed
```

Changes to `infra.yml` or `deployer/` are not picked up automatically. Copy them to
`/docker/docker-compose/trcekim` and run step 4 again.

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
  from GitHub (step 2).
- `main has no deploy/omv/trcekim.yml, skipped`: the branch does not contain the app yet.
  It deploys after the next merge.
- `SQLite Error 14: unable to open database file`: the data folder is not owned by
  1000:100 (step 1).
- `network trcekim-mail declared as external, but could not be found`: start `infra.yml` first.
- The admin page shows "Sunucuya yeniden bağlanılıyor…" (reconnecting) behind the proxy:
  enable Websockets support on the NPM proxy host.
