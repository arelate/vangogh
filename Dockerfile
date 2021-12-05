FROM golang:alpine as build
RUN apk add --no-cache --update git
ADD . /go/src/app
WORKDIR /go/src/app
RUN go get ./...
RUN go build \
    -o vangogh \
    -ldflags="-s -w \
    -X 'vangogh/version.Version=`git describe --tags --abbrev=0`' \
    -X 'vangogh/version.Commit=`git rev-parse --short HEAD`' \
    -X 'vangogh/version.BuildDate=`date +%FT%T%z`'" \
    main.go

FROM alpine
COPY --from=build /go/src/app/vangogh /usr/bin/vangogh

EXPOSE 1853
#app configuration: my-defaults.json
VOLUME /etc/vangogh
#temporary data: cookies.json
VOLUME /var/tmp
#app logs
VOLUME /var/log/vangogh
#app artifacts: checksums, images, metadata, recycle_bin, videos
VOLUME /var/lib/vangogh

ENTRYPOINT ["/usr/bin/vangogh"]
CMD ["serve","-p", "1853", "-stderr"]
