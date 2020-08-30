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
- [done] enumerate ids - index?
- add politeness support for account queries (port from vangogh)
- sort by ... - index?
- search - index?
- tests :-)

Misc.:

- [done] use connection string, not URL parts for MongoDB
- [done] add support for file as password and connection string
- think about exporting JSON for product/accountProduct/details

Download images:

- enumerate and download cover images using GOG.com sizes 1x: 196, 2x: 392
- enumerate and download screenshots using GOG.com sizes 1x: 271, 2x: 542

Download product files:

- for a given []OS/[]Language
- additionally, download XML with validation data
- store association between GOG url and resolved url path to get filename (resolvedUrls)
- validate product files
- consider downloading from validation file

CLI apps:

- fetch products, accountProducts, wishlist
- fetch details(id int)
- download image(id int)
- download gallery(id int)
- download files(id int, []os, []lang)
- validate files
- cleanup files

Server:

- create endpoints for products/accountProducts/details
- understand Go html templating