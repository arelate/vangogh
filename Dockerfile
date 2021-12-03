FROM golang:alpine as build
RUN apk add --no-cache --update git
ADD . /go/src/app
WORKDIR /go/src/app
RUN go get ./...
RUN go build \
    -o vangogh \
    -ldflags="-s -w -X 'vangogh/version.Version=`git describe --tags --abbrev=0`' -X 'vangogh/version.Commit=`git rev-parse --short HEAD`' -X 'vangogh/version.BuildDate=`date +%FT%T%z`'" \
    main.go

FROM alpine
COPY --from=build /go/src/app/vangogh /usr/bin/vangogh

EXPOSE 5000
VOLUME checksums
VOLUME downloads
VOLUME images
VOLUME logs
VOLUME metadata
VOLUME recycle_bin
VOLUME videos

ENTRYPOINT ["/usr/bin/vangogh"]
CMD ["serve","-p", "5000"]
