FROM golang:alpine as build
RUN apk add --no-cache --update git
ADD . /go/src/app
WORKDIR /go/src/app
RUN go get ./...
RUN go build \
    -o vg \
    -ldflags="-s -w \
    -X 'github.com/boggydigital/vangogh/version.Version=`git describe --tags --abbrev=0`' \
    -X 'github.com/boggydigital/vangogh/version.Commit=`git rev-parse --short HEAD`' \
    -X 'github.com/boggydigital/vangogh/version.BuildDate=`date +%FT%T%z`'" \
    main.go

FROM alpine
COPY --from=build /go/src/app/vg /usr/bin/vg

EXPOSE 1853
#app configuration: settings.txt
VOLUME /etc/vangogh
#temporary data: cookies.txt
VOLUME /var/tmp
#app logs
VOLUME /var/log/vangogh
#app artifacts: checksums, images, metadata, recycle_bin, videos
VOLUME /var/lib/vangogh

ENTRYPOINT ["/usr/bin/vg"]
CMD ["serve","-p", "1853", "-stderr"]
