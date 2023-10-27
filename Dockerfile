FROM golang:alpine as build
RUN apk add --no-cache --update git
ADD . /go/src/app
WORKDIR /go/src/app
RUN go get ./...
RUN go build \
    -a -tags timetzdata \
    -o vg \
    -ldflags="-s -w -X 'github.com/arelate/vangogh/cli.GitTag=`git describe --tags --abbrev=0`'" \
    main.go

FROM alpine:latest
COPY --from=build /go/src/app/vg /usr/bin/vg
COPY --from=build /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/

EXPOSE 1853

# cold storage is less frequently accessed data,
# that can be stored on hibernating HDD.
# hot storage is frequently accessed data,
# that can benefit from being stored on SSD.

# backups (cold storage)
EXPOSE /var/lib/vangogh/backups
# downloads (cold storage)
EXPOSE /var/lib/vangogh/downloads
# images (hot storage)
EXPOSE /var/lib/vangogh/images
# input (hot storage)
EXPOSE /var/lib/vangogh/input
# items (hot storage)
EXPOSE /var/lib/vangogh/items
# logs (cold storage)
EXPOSE /var/log/vangogh
# metadata (hot storage)
EXPOSE /var/lib/vangogh/metadata
# output (hot storage)
EXPOSE /var/lib/vangogh/output
# recycle_bin (cold storage)
EXPOSE /var/lib/vangogh/recycle_bin
# videos (cold storage)
EXPOSE /var/lib/vangogh/videos

ENTRYPOINT ["/usr/bin/vg"]
CMD ["serve","-port", "1853", "-stderr"]
