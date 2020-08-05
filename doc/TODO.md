# vangogh TODO

Project:
- establish more canonical project structure
- extract GOG code into gogapi module
- create configuration file
- identify key directories (esp. going forward for docker - want to split files/metadata)

Finish authorization:
- [done] clean up HTML element search with filter
- add tests
- [done] save/loadCookies
- [done] better captcha detection
- [started] document adding cookies from the browser

Download products/accountProducts/gameDetails:
- download whole pages and map product id to page id
- download individual game details
- download images

Download product files:
- for a given OS/Language
- additionally download XML with validation data

Server:
- create endpoints for products/accountProducts/gameDetails
- understand Go html templating