# vangogh TODO

Project:
- [done] <del>establish better project structure</del>
- [done] <del>extract GOG code into gog package</del>
- [dont] create configuration file
- [started] identify key directories (esp. going forward for docker - want to split files/metadata)

Finish authorization:
- [done] clean up HTML element search with filter
- [done] save/loadCookies
- [done] better captcha detection
- [started] document adding cookies from the browser
- add tests

Download products/accountProducts/gameDetails:
- [cut] download whole pages and map product id to page id
- [started] download individual game details
- [started] download account products
- [done] download products
- download images
- download screenshots

Download product files:
- for a given OS/Language
- additionally, download XML with validation data

Server:
- create endpoints for products/accountProducts/gameDetails
- understand Go html templating