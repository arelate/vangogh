# vangogh backlog

- fix 2FA message to use GOG.com language
- re-implement cookies using kvas
- add operations log
- check downloaded data
- track updated account products (new, modified, updated) 
- figure out throttling situation for downloading many details at once (do they send headers we can check? figure out other heuristics)
- add list objective to print ID, title
- add search objective to print ID, title

## DONE

- ~~add helper funcs to parse media. Need this to pass media to gogurls from clo~~
- ~~add "default" page gogurls that take page, media, sort for the fetch func switch flow~~
- ~~transition gogurls to use string in place of ids~~
- ~~only add hidden / updated flag if specified~~