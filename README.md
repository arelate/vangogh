# vangogh

Backend microservice to sync and serve metadata, images, videos from GOG.com. Can be used as a CLI app.

## Installation

The recommended way to install `vangogh` is with docker-compose:

- create a `docker-compose.yaml` file (this minimal example omits common settings like network, restart, etc):

```yaml
version: '3'
services:
  vangogh:
    container_name: vangogh
    image: ghcr.io/arelate/vangogh:latest
    volumes:
      # app configuration: settings.txt
      - /docker/vangogh:/etc/vangogh
      # temporary data: cookies.txt, exported metadata
      - /docker/vangogh:/var/tmp
      # app logs
      - /docker/vangogh/logs:/var/log/vangogh
      # app artifacts: checksums, images, metadata, recycle_bin, videos
      - /docker/vangogh:/var/lib/vangogh
    ports:
      # https://en.wikipedia.org/wiki/Vincent_van_Gogh
      - "1853:1853"
```
- (move it to location of your choice, e.g. `/docker/vangogh` or remote server or anywhere else)
- while in the directory with that config - pull the image with `docker-compose pull`
- start the service with `docker-compose up -d`

## Usage

Currently there is no frontend for this service and documentation is light 😞, though you can use it with a CLI interface to get and maintain all the publicly available GOG.com data, including your account data (game installers):

- in the same folder with `docker-compose.yaml` config use `docker-compose exec vangogh vg <command> <options>`
- most commonly you would run sync `docker-compose exec vangogh vg sync -all` that gets all available data from GOG.com

### Make sure you have available disk space

Please note that all data === a lot of data, so make sure you have space available or use specific options to get what you need (any combination of `-data -images -screenshots -videos -downloads`)

Here are some current estimates of how much space you'll need for each type of data:

- core metadata (Store, Account, Wishlist products and associated detailed data): 400Mb
- images, including screenshots: 35Gb
- videos: 70Gb
- checksums (automatically downloaded with installers for validation): 120Mb
- product installers: estimate is about 6.5Tb for 1000 products for installers for the 10 most common languages for all operating systems (just Windows and English is about half of that)

## REST API

The default installation method provided above would start a web service that can serve available data over HTTP REST API. 

Here are available endpoints and parameters:

- /v1/keys?product-type&media&sort&desc
- /v1/redux?property&id
- /v1/data?product-type&media&id
- /v1/images?id
- /v1/videos?id
