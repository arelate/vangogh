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

# backups (cold storage)
/var/lib/vangogh/backups
# downloads (cold storage)
/var/lib/vangogh/downloads
# images (hot storage)
/var/lib/vangogh/images
# input_files (hot storage)
/var/lib/vangogh
# items (hot storage)
/var/lib/vangogh/items
# logs (cold storage)
/var/log/vangogh
# metadata (hot storage)
/var/lib/vangogh/metadata
# output_files (hot storage)
# same as input_files
#/var/lib/vangogh
# recycle_bin (cold storage)
/var/lib/vangogh/recycle_bin
# videos (cold storage)
/var/lib/vangogh/videos

ENTRYPOINT ["/usr/bin/vg"]
CMD ["serve","-port", "1853", "-stderr"]
