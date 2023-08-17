FROM golang:alpine as build
RUN apk add --no-cache --update git
ADD . /go/src/app
WORKDIR /go/src/app
RUN go get ./...
RUN CGO_ENABLED=0 go build -a -installsuffix cgo -tags timetzdata -o vg main.go

FROM scratch
COPY --from=build /go/src/app/vg /usr/bin/vg
COPY --from=build /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/

EXPOSE 1853
#temporary data: cookies.txt
VOLUME /var/tmp
#app logs
VOLUME /var/log/vangogh
#application data root (cold store): checksums, recycle_bin, videos
VOLUME /var/lib/vangogh
#images (warm store)
VOLUME /var/lib/vangogh/images
#items (warm store)
VOLUME /var/lib/vangogh/items
#metadata (warm store)
VOLUME /var/lib/vangogh/metadata

ENTRYPOINT ["/usr/bin/vg"]
CMD ["serve","-port", "1853", "-stderr"]
