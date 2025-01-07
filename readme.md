# Overview

HAUS is a home automation system that works within your personal network. The only part of the application that requires
access to the cloud is for authentication of users. The current version is using Auth0 for authentication. However, this
may change in the future.

## Zigbee

Zigbee is an open protocol that various manufacturers are using in their "smart" devices. The current version is
targeting support for many Zigbee devices.

# System Requirements

- .NET 5
- MQTT server
    - This can be obtained on Mac OS using `brew install mosiquitto`
- Node/NPM
- Yarn

Currently, this is only intended to be run from the command line with all code available locally.

# Environment Variables

```bash
export GITHUB_TOKEN="{insert GitHub PAT w/ read access to repository}";

export AUTH_DOMAIN="{insert auth0 domain}"
export AUTH_CLIENT_ID="{insert auth0 client id}"
export AUTH_CLIENT_SECRET="{insert client secret for above client id}"
export AUTH_AUDIENCE="https://haus-portal-api.com"
export AUTH_USERNAME="{insert user name}"
export AUTH_PASSWORD="{insert user password"

export CYPRESS_AUTH_DOMAIN="${AUTH_DOMAIN}"
export CYPRESS_AUTH_CLIENT_ID="${AUTH_CLIENT_ID}"
export CYPRESS_AUTH_CLIENT_SECRET="${AUTH_CLIENT_SECRET}"
export CYPRESS_AUTH_AUDIENCE="${AUTH_AUDIENCE}"
export CYPRESS_AUTH_USERNAME="${AUTH_USERNAME}"
export CYPRESS_AUTH_PASSWORD="${AUTH_PASSWORD}"
```