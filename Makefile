SHELL := /bin/bash
COMPOSE := docker compose -f docker-compose.local.yml

WEB_HOST_DIR := src/Haus.Web.Host
SITE_HOST_DIR := src/Haus.Site.Host
ZIGBEE_HOST_DIR := src/Haus.Zigbee.Host
ACCEPTANCE_TESTS_DIR := tests/Haus.Acceptance.Tests

.PHONY: build certs publish start stop watch web-host site-host zigbee-host \
        test-unit test-acceptance docker-publish add-project migration

build:
	. ./scripts/variables.sh && dotnet build /p:Version="$$VERSION"

certs:
	./scripts/generate-dev-certs.sh

publish:
	./scripts/publish-app.sh

start: certs publish
	$(COMPOSE) up --build

stop:
	$(COMPOSE) down

watch:
	set -m; \
	trap ' \
		for p in $$(jobs -p); do kill -TERM -$$p 2>/dev/null; done; \
		sleep 2; \
		for p in $$(jobs -p); do kill -KILL -$$p 2>/dev/null; done \
	' EXIT INT TERM; \
	(cd $(WEB_HOST_DIR) && DOTNET_WATCH_RESTART_ON_RUDE_EDIT=1 dotnet watch) & \
	(cd $(SITE_HOST_DIR) && DOTNET_WATCH_RESTART_ON_RUDE_EDIT=1 dotnet watch) & \
	wait

web-host:
	cd $(WEB_HOST_DIR) && dotnet run --launch-profile acceptance

site-host:
	cd $(SITE_HOST_DIR) && dotnet run --launch-profile acceptance

zigbee-host:
	cd $(ZIGBEE_HOST_DIR) && dotnet run

test-unit:
	./scripts/run-unit-tests.sh

test-acceptance: certs publish
	$(COMPOSE) up --build -d --wait
	dotnet test $(ACCEPTANCE_TESTS_DIR) --no-restore; \
	status=$$?; \
	$(COMPOSE) down; \
	exit $$status

docker-publish:
	./scripts/publish-to-docker-hub.sh

add-project:
	./scripts/add-project.sh $(TYPE) $(NAME)

migration:
	./scripts/create-ef-migration.sh $(NAME)
