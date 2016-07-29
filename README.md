GoodOfflineGames
================

GoodOfflineGames is a command line sync service to locally download and incrementally keep in sync you account data from GOG.com. Sister project [GoodOfflineGames.html](https://github.com/boggydigital/GoodOfflineGames.html) provides HTML frontend for that data. 

Command line sync service supports:
* 2FA authentication
* Incremental updates
* Optional downloading of images
* Optional downloading of screenshots
* Tags from GOG.com account
* Serial keys
* Optional downloading your game files
* Validating your game files using same validation data GOG.com uses themselves
* Optional clearing of game folders to keep only the latest versions available

Screenshots
===========

![Commandline](https://github.com/boggydigital/GoodOfflineGames/blob/master/GoodOfflineGames/GoodOfflineGames/CMD.PNG)

Using the app
=============

Sync service - GoodOfflineGames.exe: 
* Run the binary in any folder, that might have results from previous runs 
* Service uses settings.json file and produces the following files:
* products.js - list of public products from GOG.com/games
* gamedetails.js - private accound game details from GOG.com/account, download links, etc.
* owned.js - list of owned games
* productdata.js - public product data from GOG.com/games for each product in products.js
* updated.js - list of updated games
* wishlisted.js - list of wishlisted games 
* screenshots.js - list of product screenshots
* checkedowned.js - date of last checked owned product game details (only used when updateAll is used as well)
* checkedproductdata.js - date of last checked product data. Sync service would force update product data every 30 days to account for updated GOG.com information 
* ./_images - product images files (optional)
* ./_screenshots - downloaded screenshots (optional)
* ./_md5 - downloaded validation information containing expected filename, size and computed md5 for chunks of the file

Drop release from [GoodOfflineGames.html](https://github.com/boggydigital/GoodOfflineGames.html) over the results to get HTML frontend on local data.

settings.json
=============
Settings.json allows users to specify input data. Without this file or expected values in the file the application would ask for any critical missing data (like you username and password) or fallback to defaults. Options that can specified:

* username - your GOG.com username (email)
* password - you GOG.com password (used only to authenticate current session)
* downloadLangugages - languages to download, default: ["en"], supported languages "en", "cz", "da", "de", "es", "fr", "it", "hu", "nl", "no", "pl", "pt", "br", "ro", "sk", "fi", "sv", "tr", "uk", "gk", "bl", "ru", "sb", "ar", "ko", "cn", "jp".
* downloadOperatingSystems - files for which operating systems to download, default: ["Windows"], supported operating systems "Windows", "Mac, "Linux"
* downloadProductFiles - true: download product files, false (default): don't download product files
* validateProductFiles - true: download md5 .xml file from GOG.com and validate downloaded files against that (using name, size, and md5 of each chunk) (default), false: don't perform the validation
* downloadImages - true (default): download product thumbnails, false: don't download product thumbnails
* downloadScreenshots - true: download product screenshots, false (default): don't download product screenshots
* updateAll - true: attempt to update all available products, false (default): only update product marked as updated
* cleanupProductFolders - true (default): remove all files that are not specified for current version on GOG.com to _RecycleBin (preserving folder structure), false: don't remove any files from product folders
* useLog - true (default): write command line output to the log.txt file in the current folder (append), false: don't write log
