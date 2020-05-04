Technical Debt Reduction Plan
=============================

- [done] Review all delegates and look for opportunities to optimize, especially GOG.Delegates that are almost certainly not needed
- [done] Review namespaces to consolidate "area" delegates - e.g. Data, Collections
- Consolidate ProductTypes in GOG.* namespaces
- Try to generate derrived type specific classes based on abstract classes - https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/
- Cleanup TODOs
- Resolve compiler warnings

Backend
=======

- Debug authorization controller - seems to always fail on first two attempts, succeeds on third

- Implement recordsContoller to track created/modified/deleted/started/completed for an id
    - *[done]* Track CRUD operations for index/dataControllers
    - *[done]* Track started/completed for activities (starting with PageResultUpdateActivity)
    - Reimplement updateData-updated to use the following conditions:
        1) set products that have isNew, Updates>0 as updated
        2) set products that were created/updated between "updateData-accountProducts" started and completed as updated

- change clear updated from unconditional clearing to recordsContoller based system. 
To clear updated we'll need successful completion of several activities since product was added as updated:
    - the moment product updated entry was modified is T0
    - Since T0, there should be the following events on the product timeline:
    - gameDetails activity should have been started and completed at Tgda > T0;
    - gameDetails for this product modified at Tgd > Tgda;
    - product files download should have been started and completed at Tpfd > Tgd;
    - product files validation should have been started and completed at Tv > Tpfd;

- Performance: https://stackoverflow.com/questions/45644934/notepad-beats-them-all
- Performance: rethink data controller model:
    - deprecate index controller
    - use stash for data/records
    - serialize/deserialize as a collection using commit model
    - binary serialization by default, option to export json for items and slices
    - enumerate by recorded
    - add export operation to produce json files

- modify (again) console output controller to use "single line control" and output only on one line.
    - provide same multi-line viewModel
    - combine that into single line, omiting some less critical information (consider hints to mark that)
    - return colored output and drive through templates
    - provide two methods in the output controller: writeLine (output a line and start a new line) and rewrite (CR + write)

- continue to add .editorconfig rules
    - https://raw.githubusercontent.com/dotnet/samples/master/.editorconfig
    - https://groups.google.com/forum/#!topic/editorconfig/Ftaui8OlLYc
    - https://www.techcartnow.com/enforcing-c-sharp-7-code-style-latest-coding-patterns-using-editorconfig-in-visual-studio-2017/
    - https://docs.microsoft.com/en-us/visualstudio/ide/editorconfig-code-style-settings-reference

- investigate /user/data/games (from https://www.gog.com/forum/general/unofficial_gog_api_documentation/page2) and game_type == pack specifically

- rearchitecture commands, parameters, targets into a new sessionContext class
    - create rich command line interface that takes separate commands, parameters and target ids and expands this into an operation queue
    - consider to port additional flags from settings (user, password, OS, language, etc.)
    - transition settings to sessionDefaults 
    - add commands: help, search

- Technical debt
    - complete splitting delegates from controllers
    - change functional controllers to work on product model type and have activities loop updates using those 
    - unit tests
    - move to a cleaner folder structure: /docs, /src, etc.

- introduce repair controllers - chunks repair (download by chunk, validate by chunk); validationFileIsNotValid (see if XML can't be loaded and do simple text transforms for known issues)

- [Backlog] consider whitelists, blacklists for product operations

- [Backlog] serve data and frontend files via http or better yet websockets server

- [Backlog] Few improvements from https://blogs.msdn.microsoft.com/dotnet/2018/11/12/building-c-8-0/ (async iterators)

- debug data validation - why stored file md5 doesn't match precomputed hash at push (encoding? special characters?)

Frontend
========

- es6 modules, fetch, promises, custom properties
- move to a new data model with data controllers and dynamic data loading
- Use notifications API on web client view on updates
- motion
- mobile layouts
