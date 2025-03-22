# vangogh

A service to sync and serve games and data from GOG.com. Can be used as a CLI app or a web service that provides a frontend to browse, search that data.

| Dark theme                                       | Light theme                                        |
|--------------------------------------------------|----------------------------------------------------|
| ![List Dark](github_images/list-dark.jpeg)       | ![List Light](github_images/list-light.jpeg)       |
| ![Product Dark](github_images/product-dark.jpeg) | ![Product Light](github_images/product-light.jpeg) |


## Future vangogh ideas and aspirations

`vnagogh` roadmap is available [here](ROADMAP.md).

## Installation

The recommended way to install `vangogh` is with docker-compose:

- create a `docker-compose.yaml` file (this minimal example omits common settings like network, restart, etc):

```yaml
version: '3'
services:
  vangogh:
    container_name: vangogh
    image: ghcr.io/arelate/vangogh:latest
    environment:
    # Set operating systems, languages, etc - customize to your needs
      - VANGOGH_OS=Windows # possible values: Windows, macOS, Linux
      - VANGOGH_LANG-CODE=en # see all possible values in https://github.com/arelate/southern_light/blob/main/gog_integration/languages.go  
      - VANGOGH_NO-PATCHES=true # this disables individual patches downloads
    # Uncomment (remove leading # in the line below) to create a log file on every sync 
    # - VANGOGH_SYNC_DEBUG=true    
    volumes:
      # cold storage - less frequently accessed data, that can be stored on hibernating HDD.
      # hot storage - frequently accessed data, that can benefit from being stored on SSD.
      # backups (cold storage)
      - /docker/vangogh/backups:/var/lib/vangogh/backups
      # downloads (cold storage)
      - /docker/vangogh/downloads:/var/lib/vangogh/downloads
      # checksums (hot storage)
      - /docker/vangogh/checksums:/var/lib/vangogh/checksums
      # images (hot storage)
      - /docker/vangogh/images:/var/lib/vangogh/images
      # input (hot storage)
      - /docker/vangogh:/var/lib/vangogh/input
      # description_images (hot storage)
      - /docker/vangogh/description_images:/var/lib/vangogh/description_images
      # logs (cold storage)
      - /docker/vangogh/logs:/var/lib/vangogh/logs
      # metadata (hot storage)
      - /docker/vangogh/metadata:/var/lib/vangogh/metadata
      # output (hot storage)
      - /docker/vangogh:/var/lib/vangogh/output
      # recycle_bin (cold storage)
      - /docker/vangogh/recycle_bin:/var/lib/vangogh/recycle_bin
      # sharing timezone from the host
      - /etc/localtime:/etc/localtime:ro
      # certificates
      - /etc/ssl/certs/ca-certificates.crt:/etc/ssl/certs/ca-certificates.crt:ro
    ports:
      # https://en.wikipedia.org/wiki/Vincent_van_Gogh
      - "1853:1853"
```
- (move it to location of your choice, e.g. `/docker/vangogh` or remote server or anywhere else)
- while in the directory with that config - pull the image with `docker-compose pull`
- start the service with `docker-compose up -d`
- assuming everything was setup correctly - `vangogh` will now be available at this location address, port 1853 (e.g. http://localhost:1853 for a local server installation)

## Browser compatibility

`vangogh` has been tested in the most popular browsers (Chrome and other Chromium-based browsers, Safari, Firefox) and should work reasonably well in all of them with minor visual differences.

However, in order to make `vangogh` work in Firefox you'd need to enable [@scope](https://developer.mozilla.org/en-US/docs/Web/CSS/@scope) support if you're comfortable enabling preview Web Platform feature. Here's how to do that (copied from MDN): 

```text
From version 128: this feature is behind the layout.css.at-scope.enabled preference (needs to be set to true). To change preferences in Firefox, visit about:config.
```


## Getting started

After you've installed `vangogh`, you need to authenticate GOG.com access by providing browser cookie data. To do that you need to create `cookies.txt` in the `temporary data` folder (see [docker installation](#Installation)),
   then follow [instructions here](https://github.com/boggydigital/coost#copying-session-cookies-from-an-existing-browser-session) to copy `gog.com` cookies into that file. When you run `vangogh` for the first time, it'll import the cookie header value and split individual parameters.

Regardless of how you do it, the content of `cookies.txt` should look like this:

```text
gog.com
 cart_token=(some value)
 gog-al=(some value)
 gog_lc=(some value)
 gog_us=(some value)
```

If you want to de-authorize `vangogh` from accessing your GOG.com data - delete the `cookies.txt` file. After that you'll still be able to download anything that does not require account authorization. All the account specific data you'll have accumulated until that point will be preserved.

## Updating data

To update your data you can use `vangogh` with a CLI interface to get and maintain all the publicly available GOG.com data, including your account data (game installers):

- in the same folder with `docker-compose.yaml` config use `docker-compose exec vangogh vangogh <command> <options>`
- most commonly you would run sync `docker-compose exec vangogh vangogh sync -all` that gets all available data from GOG.com. 

Sync is optimized to get as little data as possible on each run - only the newly added images and updated installers. There is no great way to determine if metadata was updated remotely, so all of it is fetched on each sync - however upon doing that `vangogh` would know exactly what changed and use this information to optimize decisions. You can also create scheduled runs of synchronization (e.g. with cron) to keep your data fresh. We recommend running sync no more often than every 24 hours. 

### Disk space requirements

Please note that all data === a lot of data, so make sure you have space available or use specific options to get what you need (any combination of `-data -images -screenshots -thumbnails -downloads`)

Here are few estimates of how much space you'll need for each type of data:

- metadata (GOG, Steam, PCGamingWiki, HLTB, ProtonDB data for GOG products): 1Gb
- images, including screenshots: 68Gb
- description items: 19Gb
- product installers estimates (done with `size` command): 
  - 6.5Tb/1000 products for all operating systems (Windows, macOS, Linux) for the 10 most common languages installers, DLCs, extras
  - 4.5Tb/1000 products for all operating systems (Windows, macOS, Linux), English installers, DLCs, extras 
  - 3.3Tb/1000 products for Windows, English installers, DLCs, extras
  - 1.0Tb/1000 products for macOS, English installers, DLCs, extras
  - 0.6Tb/1000 products for Linux, English installers, DLCs, extras

`vangogh` directories allow you to use the best storage type for each data type. For example you might want to store installers, checksums and logs on slower HDDs as those are typically written and read sequentially. Metadata and images on the other hand would benefit from faster SSDs if you're using `vangogh` as a web frontend for your data.

## Taking care of your data

Syncing data keeps it in sync with GOG.com. As part of this process some data is left on the system and `vangogh` provides few ways to clean up and vet the data:

- `cleanup` will take care of older downloads versions. `cleanup -all -test` will enumerate all installers that are not linked to most current data. `cleanup -all` (without `-test`) will move that stale data to `recycle_bin` under your state directory
- `vet` will take care of various data problems, such as metadata that was downloaded earlier, and is not longer available at GOG.com. `vet -all` will run series of tests of data and print out recommendations. `vet -all -fix` will also attempt to repair the data.
- `validate` will test installers you've downloaded using validation files provided by GOG.com. `validate -all` will do that for all installers (a very long process if you have a lot of them!). Please note that GOG.com is missing validation files for some installers and this will not be considered a critical error. 

## Sharing games

`vangogh` assumes you follow GOG.com [games sharing guidelines](https://support.gog.com/hc/en-us/articles/212184489-Can-I-share-games-with-others-?product=gog). Just like GOG.com, we trust you that this will not be abused.
