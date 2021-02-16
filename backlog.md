# vangogh backlog

- add missing flag to download images
- use log.Print for errors and info, fmt.Print progress 
- add module that produced log.Print to the log output
- bug "failed to fill arguments default values" when run with no arguments
- add bulk Set operation to kvas
- add protobuf support for local types in addition to JSON  
- index files should be protobuf (so no index selection)
- Bug: "list store-p" is not an error
- Store-products are stored in "products"
- split vangogh types into a separate module
- split vangogh URLs into a separate module
- add operations log
- print report at the end of operations - changed, new
- track updated account products (new, modified, updated)
- figure out throttling situation for downloading many details at once (do they send headers we can check? figure out other heuristics)
- add search objective to print ID, title
- download product types: images, screenshots, videos, product files
- "sync" objective that does all remote data fetching in one command

## DONE

- ~~add helper funcs to parse media. Need this to pass media to gog_urls from clo~~
- ~~add "default" page gog_urls that take page, media, sort for the fetch func switch flow~~
- ~~transition gog_urls to use string in place of ids~~
- ~~only add hidden / updated flag if specified~~
- ~~fix 2FA message to use GOG.com language~~
- ~~re-implement cookies using kvas~~
- ~~split fetched paginated media~~
- ~~fetch missing details with delay between downloads~~
- ~~add list objective to print ID, title~~
- ~~check downloaded data (details) to fill types TODOs~~
- ~~add "id..." parameter to "list" to be used for status updates~~
- ~~investigate "list wishlist" issues~~
  - ~~root caused to not closing files after a kvas.Get~~
  - ~~convert all returned io.Reader to io.ReadCloser (should work with remote kvas as well)~~
  - ~~use file.Create instead of OpenFile for kvas.Set~~
- ~~kvas separate index extension from content extension (allow protobuf index, png files)~~
- ~~don't use global httpClient in cmds~~ 
- ~~create NewLocalJsonClient and NewLocalProtoClient that only take URL~~

