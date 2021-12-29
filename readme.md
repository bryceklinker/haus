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