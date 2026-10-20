FROM docker:29.0.4-cli
RUN apk add --no-cache git openssh-client tzdata
COPY deploy.sh /usr/local/bin/deploy.sh
ENTRYPOINT ["/bin/sh", "/usr/local/bin/deploy.sh"]
