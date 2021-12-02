FROM golang:alpine AS build
RUN apk add --no-cache --update git
ADD . /go/src/app
WORKDIR /go/src/app
RUN go build \
    -o vangogh \
    -ldflags="-s -w -X 'vangogh/version.Version=`git describe --tags --abbrev=0`' -X 'vangogh/version.Commit=`git rev-parse --short HEAD`' -X 'vangogh/version.BuildDate=`date +%FT%T%z`'" \
    main.go

FROM alpine:latest

EXPOSE 5000
RUN apk --no-cache add ca-certificates tzdata
COPY --from=build /go/src/app/vangogh /usr/bin/vangogh
USER nobody
CMD ["/usr/bin/vangogh serve -p 5000"]