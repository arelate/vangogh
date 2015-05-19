GoodOfflineGames
================

Note: this is a modern version of GOGService that uses current GOG.com site APIs. Using this opportunity I've removed most of the redundant code. Application is now structured to support further development and easier support. 

![Screenshot](https://github.com/boggydigital/GoodOfflineGames/blob/master/GoodOfflineGames/GoodOfflineGames/screenshot.png)
HTML front end for offline GOG.com collection (games only at this time). Supports incremental updates and caching of images  and serial keys. Cross-platform frontend to downloaded products that looks good, supports search and stores some critical data offline (serial keys). It doesn't have any additional dependecies - having your disk with your collection and this data is all you need to use this on any PC that has a browser.

Usage
=====
Run the binary in any folder (this folder might have results from previous runs). This binary uses settings.json and produces data.js file, ./_images/<image files> and default.html. 

To run this on systems that doesn't have .NET (Mac, Linux) - use Mono (http://www.mono-project.com/)

settings.json
=============
This is an option file that you can use to specify input data and change parameters. Without this file the application would ask for any critical missing data (like you username and password). These are the options you can specify:

* username - your GOG.com username (email)
* password - you GOG.com password (not saved between runs, used only to authenticate current session)

Notes and further planned improvements
======================================
* The app can use GalaxyAPI that Galaxy client uses and validate checksums on the local files on demand (or on schedule)
* While performaing file validation the app can optionally check if you have older versions of the same installers and suggest to remove them to save space

