# vangogh backlog

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

- ~~add helper funcs to parse media. Need this to pass media to gogurls from clo~~
- ~~add "default" page gogurls that take page, media, sort for the fetch func switch flow~~
- ~~transition gogurls to use string in place of ids~~
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
