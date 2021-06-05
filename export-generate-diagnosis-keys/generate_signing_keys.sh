#!/bin/sh

docker build -t keiji/exposure-notifications-server $1

docker run --rm -v `pwd`/work:/work keiji/exposure-notifications-server \
	/bin/sh -c "cd /work && openssl ecparam -genkey -name prime256v1 -noout -out private.pem"

docker run --rm -v `pwd`/work:/work keiji/exposure-notifications-server \
	/bin/sh -c "cd /work && openssl ec -in private.pem -pubout -out public.pem"
