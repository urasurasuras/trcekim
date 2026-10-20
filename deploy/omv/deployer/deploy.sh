#!/bin/sh
# Polls GitHub and redeploys a TRÇekim environment whenever its branch moves
# (a merged PR or a direct push).
#
# ENVIRONMENTS lists "<compose project>:<branch>" pairs. Each project is built from a
# checkout of its branch with that branch's deploy/omv/trcekim.yml and
# $CONFIG_DIR/<project>.env. If a commit fails to build or start, the previous
# container keeps running, and that commit is not retried until the branch moves again.
set -u

: "${REPO_URL:?REPO_URL is required}"
: "${ENVIRONMENTS:?ENVIRONMENTS is required}"
POLL_SECONDS="${POLL_SECONDS:-60}"
STATE_DIR="${STATE_DIR:-/state}"
CONFIG_DIR="${CONFIG_DIR:-/config}"
COMPOSE_FILE="deploy/omv/trcekim.yml"

# The deploy key is read-only; GitHub's host key is pinned in ssh/known_hosts.
export GIT_SSH_COMMAND="ssh -i $STATE_DIR/ssh/id_ed25519 -o IdentitiesOnly=yes -o UserKnownHostsFile=$STATE_DIR/ssh/known_hosts -o StrictHostKeyChecking=yes"

log() { echo "$(date '+%F %T') $*"; }

deploy() {
    project=$1
    branch=$2
    src="$STATE_DIR/src/$project"
    env_file="$CONFIG_DIR/$project.env"

    [ -d "$src/.git" ] || git init -q "$src" || return 1
    if ! git -C "$src" fetch -q --depth 1 "$REPO_URL" "refs/heads/$branch"; then
        log "$project: fetching $branch failed"
        return 1
    fi
    sha=$(git -C "$src" rev-parse FETCH_HEAD)
    [ "$sha" = "$(cat "$STATE_DIR/$project.deployed" 2>/dev/null)" ] && return 0
    [ "$sha" = "$(cat "$STATE_DIR/$project.failed" 2>/dev/null)" ] && return 0

    git -C "$src" checkout -q --force --detach FETCH_HEAD && git -C "$src" clean -qfdx || return 1
    log "$project: deploying $branch @ $(git -C "$src" log -1 --format='%h %s')"

    if [ ! -f "$src/$COMPOSE_FILE" ]; then
        log "$project: $branch has no $COMPOSE_FILE, skipped"
        echo "$sha" > "$STATE_DIR/$project.failed"
        return 1
    fi
    if [ ! -f "$env_file" ]; then
        log "$project: missing $env_file, skipped"
        return 1
    fi

    set -- docker compose -f "$src/$COMPOSE_FILE" -p "$project" --env-file "$env_file"
    if "$@" build --pull && "$@" up -d --remove-orphans; then
        echo "$sha" > "$STATE_DIR/$project.deployed"
        rm -f "$STATE_DIR/$project.failed"
        docker image prune -f --filter label=app=trcekim > /dev/null
        log "$project: deployed"
    else
        echo "$sha" > "$STATE_DIR/$project.failed"
        log "$project: FAILED, previous version keeps running"
    fi
}

mkdir -p "$STATE_DIR/src"
log "watching $ENVIRONMENTS every ${POLL_SECONDS}s"
while :; do
    for pair in $ENVIRONMENTS; do
        deploy "${pair%%:*}" "${pair#*:}"
    done
    sleep "$POLL_SECONDS"
done
