# vangogh TODO

Project:
- [done] establish better project structure
- [done] extract GOG code into gog package
- [dont] create configuration file
- [started] identify key directories (esp. going forward for docker - want to split files/metadata)

Finish authorization:
- [done] clean up HTML element search with filter
- [done] save/loadCookies
- [done] better captcha detection
- [started] document adding cookies from the browser
- tests :-)

Download products/accountProducts/gameDetails:
- [cut] download whole pages and map product id to page id
- [done] download individual game details
- [done] download account products
- [done] download wishlist games
- [done] download products
- add changes - timestamps when product was fetched, added, updated, etc.
- enumerate and download cover images using GOG.com sizes 1x: 196, 2x: 392
- enumerate and download screenshots using GOG.com sizes 1x: 271, 2x: 542
- add politeness support for account queries (port from vangogh) 
- add some politeness for other gog.com queries (fetched >= 24 hours for example)
- tests

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