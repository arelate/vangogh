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
      - /docker/vangogh:/etc/vangogh:ro
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

The recommended way to enjoy the data sync'd by `vangogh` is [arelate/gaugin](https://github.com/arelate/gaugin). `gaugin` is a read-only view and doesn't change the data. To update your data you can use `vangogh` with a CLI interface to get and maintain all the publicly available GOG.com data, including your account data (game installers):

- in the same folder with `docker-compose.yaml` config use `docker-compose exec vangogh vg <command> <options>`
- most commonly you would run sync `docker-compose exec vangogh vg sync -all` that gets all available data from GOG.com. Sync is optimized to get as little data as possible on each run - only the newly added images, videos and updated installers. There is no great way to determine if metadata was updated remotely, so all of it is fetched on each sync - however upon doing that `vangogh` would know exactly what changed and use this information to optimize decisions.

### Disk space requirements

Please note that all data === a lot of data, so make sure you have space available or use specific options to get what you need (any combination of `-data -images -screenshots -videos -downloads`)

Here are some current estimates of how much space you'll need for each type of data:

- core metadata (Store, Account, Wishlist products and associated detailed data): 400Mb
- images, including screenshots: 30Gb
- description items: 6Gb
- videos: 73Gb
- checksums (automatically downloaded with installers for validation): 120Mb
- product installers: estimate is about 6.5Tb for 1000 products for installers for the 10 most common languages for all operating systems (just Windows and English is about half of that)

## REST API

The default installation method provided above would start a web service that can serve available data over HTTP REST API. 

Here are available endpoints and parameters:

- /v1/all_redux?property&product-type&media
- /v1/data?product-type&media&id
- /v1/downloads?id&operating-system&language-code
- /v1/keys?product-type&media&sort&desc
- /v1/redux?property&id
- /v1/search?text&(searchable properties)

All endpoints support only GET requests and return [gob data](https://go.dev/blog/gob) (format parameter can be used to request JSON). No endpoint provides access to digital artifacts (images, videos, installers), since it's not effective to do that for `vangogh`. Given those files would need to be served by the front-end, you'd need access to the complete file payload in order to serve is efficiently (e.g. HTTP range requests). Instead, `vangogh` is focused on the metadata only and doesn't require authentication for any endpoint.

## Taking care of your data

Syncing data keeps it in sync with GOG.com. As part of this process some data is left on the system and `vangogh` provides few ways to clean up and vet the data:

- `cleanup` will take care of older downloads versions. `cleanup -all -test` will enumerate all installers that are not linked to most current data. `cleanup -all` (without `-test`) will move that stale data to `recycle_bin` under your state directory
- `vet` will take care of various data problems, such as metadata that was downloaded earlier, and is not longer available at GOG.com. `vet -all` will run series of tests of data and print out recommendations. `vet -all -fix` will also attempt to repair the data.
- `validate` will test installers you've downloaded using validation files provided by GOG.com. `validate -all` will do that for all installers (a very long process if you have a lot of them!). Please note that GOG.com is missing validation files for some installers and this will not be considered a critical error. 

## Sharing games

`vangogh` assumes you follow GOG.com [games sharing guidelines](https://support.gog.com/hc/en-us/articles/212184489-Can-I-share-games-with-others-?product=gog). Just like GOG.com, we trust you that this will not be abused.
