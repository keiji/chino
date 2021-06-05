FROM golang:1.16 AS builder
ENV EXPOSURE_NOTIFICATIONS_SERVER_VERSION v0.29.0

MAINTAINER Cappuccino Authors

RUN git clone https://github.com/google/exposure-notifications-server.git \
	&& cd exposure-notifications-server \
	&& git checkout refs/tags/$EXPOSURE_NOTIFICATIONS_SERVER_VERSION

WORKDIR exposure-notifications-server
RUN go run ./tools/export-generate --help
