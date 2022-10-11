Known get-items errors RCA
==========================

## 2034949552 Cult of the Lamb

Has typo in the URL: https://items.gog.com/ult_of_the_lamb/COTL_Boss.webm, works on GOG thanks to data-fallbackurl="/items/cult_of_the_lamb/COTL_Boss.gif" attribute that is used to replace this with .gif

## 1928341659 Shardpunk: Verminfall

api-products-v1 and api-products-v2 contain the following links:

- https://items.gog.com/shardpunk_verminfall/1.gif
- https://items.gog.com/shardpunk_verminfall/2.gif
- https://items.gog.com/shardpunk_verminfall/3.gif

While https://www.gog.com/en/game/shardpunk_verminfall has the following links (note double digits):

- https://items.gog.com/shardpunk_verminfall/11.gif
- https://items.gog.com/shardpunk_verminfall/22.gif
- https://items.gog.com/shardpunk_verminfall/33.gif

It's possible GOG.com is using some other data source that's not sharing data with api-products-v1, api-products-v2

## 1149708190 Shardpunk: Verminfall Demo

Exactly the same files and problem as 1928341659 Shardpunk: Verminfall above

## 1295420179 BEAUTIFUL DESOLATION

api-products-v1 and api-products-v2 contain link to https://items.gog.com/beautiful_desolation/BD_Logo_gif62.gif, while GOG.com store page uses https://items.gog.com/beautiful_desolation/BD_Logo_gif6.gif along with some other item links not present in data available to vangogh: e.g. https://items.gog.com/beautiful_desolation/ratings.png

Potentially the same issue as 1928341659 Shardpunk: Verminfall and different source data used to generate the store page.

## 1778772155 Deep Sky Derelicts: New Prospects

https://items.gog.com/deep_sky_derelicts_-_new_prospects/dsd-locations-conditions.mp4 returns 404 on gaugin and GOG.com and the video is missing for both.

## 1186796445 Talisman - Base Game: Legendary Deck 
https://items.gog.com/talisman_-_base_game_legendary_deck/Base-Game-Banner.png returns 404 on gaugin and GOG.com and the image is missing for both.

## 1159265992 Sandwalkers

Missing items in Safari are in .webm format (work find in Chrome). The middle item is missing on gaugin, but displayed as https://items.gog.com/sandwalkers/Gif_Mainfeatures.gif on GOG.com and is coming from data-fallbackurl: <video autoplay loop muted playsinline data-fallbackurl="https://items.gog.com/sandwalkers/Gif_Mainfeatures.gif" > <source src="https://items.gog.com/sandwalkers/mainfeatures.gif" type="video/webm"> </video>. Note: vangogh downloads both .gif files and currently has no support for fallback content on invalid video sources.