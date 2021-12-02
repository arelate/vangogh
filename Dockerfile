FROM golang:alpine
RUN mkdir /app
ADD . /app/
WORKDIR /app
RUN go mod download && go build -o vangogh .

EXPOSE 5000
VOLUME checksums
VOLUME downloads
VOLUME images
VOLUME logs
VOLUME metadata
VOLUME recycle_bin
VOLUME videos

ENTRYPOINT ["./vangogh"]
CMD ["serve","-p", "5000"]