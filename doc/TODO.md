# vangogh TODO

Project:
- [done] establish better project structure
- [done] extract GOG code into gog package
- [done] create configuration file
- [done] identify key directories (esp. going forward for docker - want to split files/metadata)

Finish authorization:
- [done] clean up HTML element search with filter
- [done] save/loadCookies
- [done] better captcha detection
- [started] document adding cookies from the browser
- tests :-)

Download metadata:
- [cut] download whole pages and map product id to page id
- [done] download individual game details
- [done] download account products
- [done] download wishlist games
- [done] download products
- [done] add changes - timestamps when product was created, modified, etc.
- [done] enumerate changes in the interval (after, until)
- [done] parse filenames (that changes track) back to id, media type 
- [started] enumerate ids - index?
- add politeness support for account queries (port from vangogh) 
- add some politeness for other gog.com queries (fetched >= 24 hours for example)
- enumerate and download cover images using GOG.com sizes 1x: 196, 2x: 392
- enumerate and download screenshots using GOG.com sizes 1x: 271, 2x: 542
- sort by ... - index?
- search - index?
- tests :-)

Download product files:
- for a given OS/Language
- additionally, download XML with validation data
- validate product files, consider downloading from validation file

Open issues:
- Index files for enumeration (files in a dir?)
- Search

CLI apps:
- ?

Server:
- create endpoints for products/accountProducts/gameDetails
- understand Go html templating