# Throwaway systemd-as-PID1 container used only to prove the .deb installs
# cleanly, without touching the host's real systemd/apt state or its running
# HAUS containers. Runs its own nested Docker daemon (via the docker.io
# package's systemd unit) so container names/ports never collide with
# anything on the host.
FROM ubuntu:24.04

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y \
      systemd systemd-sysv docker.io docker-compose-v2 openssl curl iproute2 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# vfs, not the default overlayfs -- this container's own Docker layers
# already sit on the host's overlayfs, and overlay-on-overlay reliably
# fails here ("failed to mount ... invalid argument"). vfs is slower but
# is the storage driver that actually works nested this deep.
RUN mkdir -p /etc/docker && echo '{"storage-driver": "vfs"}' > /etc/docker/daemon.json

STOPSIGNAL SIGRTMIN+3
CMD ["/lib/systemd/systemd"]
