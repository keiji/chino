#!/bin/sh

docker build -t keiji/exposure-notifications-server $1

echo "Generating diagnosis-keys from $2"

docker run --rm -v `pwd`/work:/work keiji/exposure-notifications-server \
	/bin/sh -c "go run ./tools/export-generate \
	    --signing-key=/work/private.pem \
	    --tek-file=/work/$2 \
	    --region=440 \
	    --key-id=440 \
	&& cp /tmp/*.zip /work/"
