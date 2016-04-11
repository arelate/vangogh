/// <reference path="./selectionController.js" />
/// <reference path="./templateController.js" />
/// <reference path="./viewController.js" />
/// <reference path="./mnemonicsController.js" />

var Images = function() {
    var thumbnails = [];
    var getImageLocalUri = function(absoluteUri) {
        var imageParts = absoluteUri.split("/");
        return imageParts[imageParts.length - 1];
    }
    var getRelativeUri = function(absoluteUri) {
        var imageLocalUri = getImageLocalUri(absoluteUri);
        return {
            "productImage": "_images/" + imageLocalUri + "_800.jpg",
            "productRetinaImage": "_images/" + imageLocalUri + ".jpg",
            "screenshot": "_screenshots/" + imageLocalUri
        }
    }
    var getThumbnailSrc = function(absoluteUri) {
        var imageLocalUri = getImageLocalUri(absoluteUri);
        return "_images/" + imageLocalUri + "_196.jpg 1x, " +
            "_images/" + imageLocalUri + "_392.jpg 2x";
    }
    var hideOnError = function(img) {
        if (img && img.classList) img.classList.add("hidden");
    }
    var updateProductThumbnails = function(container) {
        var productThumbnails = container.querySelectorAll(".product img[data-src]");

        for (var ii = 0; ii < productThumbnails.length; ii++) {
            var image = productThumbnails[ii];
            if (!image) break;
            var source = image.getAttribute("data-src");
            image.srcset = getThumbnailSrc(source);
            image.removeAttribute("data-src");
            image.classList.remove("hidden");
        }
    }
    return {
        "getRelativeUri": getRelativeUri,
        "getThumbnailSrc": getThumbnailSrc,
        "updateProductThumbnails": updateProductThumbnails,
        "hideOnError": hideOnError
    }
} ();

var GameDetails = function() {
    var pIndex;
    var pdIndex;
    var gdIndex;
    var selectionController;
    var viewController;
    var gameDetailsContainer;
    var showDLC = function(id) {
        // add DLC. Showing only 1st level of DLCs to avoid recursion
        var dlcContent = [];
        var dlcProducts = [];
        var processedDlcs = [];

        var pd = pdIndex && pdIndex.getElementByKey(id);
        if (pd && pd.dlcs)
            for (var ii = 0; ii < pd.dlcs.length; ii++) {
                dlcProducts.push(pd.dlcs[ii]);
                // don't add DLCs that don't have id
                if (pd.dlcs[ii].id > 0)
                    processedDlcs.push(pd.dlcs[ii].title);
            }

        var gd = gdIndex ? gdIndex.getElementByKey(id) : undefined;
        if (gd && gd.dlcs)
            for (var ii = 0; ii < gd.dlcs.length; ii++) {
                if (processedDlcs.indexOf(gd.dlcs[ii].title) > -1) continue;
                dlcProducts.push(gd.dlcs[ii]);
                // don't add DLCs that don't have id
                if (gd.dlcs[ii].id > 0)
                    processedDlcs.push(gd.dlcs[ii].title);
            }


        for (var ii = 0; ii < dlcProducts.length; ii++)
            dlcContent.push(viewController.createView(dlcProducts[ii], ViewModelProvider.getGameDetailsViewModel, "gameDetails"));

        return viewController.createView(dlcContent, ViewModelProvider.getDLCsViewModel, "dlc");
    }
    var show = function(id) {
        var product = pIndex.getElementByKey(id);
        if (!product) return;

        var markup = "";

        var htmlView = viewController.createView(product, ViewModelProvider.getGameDetailsViewModel, "gameDetails");

        markup = htmlView;
        markup += showDLC(id);

        // show bundled products
        if (Bundles.isParent(id)) {
            var bundleContent = [],
                bundleClass;

            var bundle = Bundles.getBundleByParentId(id);
            for (var ii = 0; ii < bundle.children.length; ii++) {
                var product = pIndex.getElementByKey(bundle.children[ii]);

                var productView = viewController.createView(product, ViewModelProvider.getGameDetailsViewModel, "getGameDetails");
                var dlcView = showDLC(bundle.children[ii]);

                bundleContent.push(productView + dlcView);
            }

            var bundleView = viewController.createView(bundleContent, ViewModelProvider.getBundleViewModel, "bundle");

            markup += bundleView;
        }

        gameDetailsContainer.innerHTML = markup;
        gameDetailsContainer.classList.remove("hidden");
        gameDetailsContainer.focus();

        Mnemonics.init(gameDetailsContainer);

        // Reset scroll position between showing details for different products
        // to resolve annoying behavior where you open details and already have
        // scroller at some >0 position
        document.body.scrollTop = 0;
    }
    var close = function() {
        gameDetailsContainer.classList.add("hidden");
    }
    var init = function(pi, pdi, gdi, sc, vc) {
        pIndex = pi;
        pdIndex = pdi;
        gdIndex = gdi;
        selectionController = sc;
        viewController = vc;
            
        gameDetailsContainer = selectionController.getById("gameDetailsContainer");
    }
    return {
        "init": init,
        "show": show,
        "close": close
    }
} ();

var SearchIndex = function() {
    var pIndex, pdIndex, gdIndex, oIndex, uIndex, wIndex;
    var searchIndex = [];
    var indexProduct = function(p) {
        var parts = [];

        if (Bundles.isChild(p.id)) return;
        var pd = pdIndex && pdIndex.getElementByKey(p.id);
        var gd = gdIndex && gdIndex.getElementByKey(p.id);
        parts.push(p.title.toLowerCase());

        if (pd) {
            if (DLC.isDLC(pd)) parts.push("dlc");
            if (pd.publisher) parts.push(pd.publisher.name.toLowerCase());
            if (pd.developer) parts.push(pd.developer.name.toLowerCase());
            if (pd.series && pd.series.name) parts.push(pd.series.name.toLowerCase());
            if (pd.genres) {
                for (var gg = 0; gg < pd.genres.length; gg++) {
                    parts.push(pd.genres[gg].name.toLowerCase());
                }
            }
        }

        if (gd && gd.tags && gd.tags.length)
            for (var ii = 0; ii < gd.tags.length; ii++)
                parts.push(gd.tags[ii].name.toLowerCase());

        if (p.worksOn.Windows) parts.push("windows");
        if (p.worksOn.Mac) parts.push("mac osx");
        if (p.worksOn.Linux) parts.push("linux");

        if (uIndex && uIndex.check(p.id)) parts.push("updated");
        if (oIndex && oIndex.check(p.id)) parts.push("owned");
        if (wIndex && wIndex.check(p.id)) parts.push("wishlisted");

        if (Bundles.isParent(p.id)) {
            // add child items title
            var bundle = Bundles.getBundleByParentId(p.id);
            if (bundle)
                for (var ii = 0; ii < bundle.children.length; ii++) {
                    var bundleChild = pIndex.getElementByKey(bundle.children[ii]);
                    if (bundleChild) parts.push(bundleChild.title.toLowerCase());
                    // add bundle children tags
                    var bundleGd = gdIndex.getElementByKey(bundle.children[ii]);
                    if (bundleGd && bundleGd.tags && bundleGd.tags.length)
                        for (var tt = 0; tt < bundleGd.tags.length; tt++)
                            parts.push(bundleGd.tags[tt].name.toLowerCase());
                }
        }
        searchIndex.push(parts.join(" "));

    }
    var init = function(pi, pdi, gdi, oi, ui, wi) {
        pIndex = pi;
        pdIndex = pdi;
        gdIndex = gdi;
        oIndex = oi;
        uIndex = ui;
        wIndex = wi;
        for (var ii = 0; ii < pi.getLength(); ii++) {
            indexProduct(pi.getElementByIndex(ii));
        }
    };
    var match = function(text) {
        var textParts = text.split(" ");
        var matchingIds = [];
        for (var ii = 0; ii < searchIndex.length; ii++) {
            if (!searchIndex[ii]) continue;
            var matches = true;
            for (var tt = 0; tt < textParts.length; tt++) {
                if (!textParts[tt]) continue;
                var exclude = textParts[tt].indexOf("-") === 0;
                if (exclude && textParts[tt].length > 1) {
                    if (searchIndex[ii].indexOf(textParts[tt].substr(1)) > -1) {
                        matches = false;
                        break;
                    }
                } else {
                    matches &= searchIndex[ii].indexOf(textParts[tt]) > -1;
                }
            }
            if (matches) matchingIds.push(ii);
        }
        return matchingIds;
    };
    return {
        "init": init,
        "match": match
    }
} ();

var Search = function() {
    var pIndex;
    var pdIndex;
    var productElements = undefined;
    var selectionController;
    var searchResults;
    var productsContainer;
    var showAllResultsLink;
    var resultsCount;
    var searchResultsLimit = 25;
    var search = function(text, showAllResults) {

        if (productElements === undefined) {
            productElements = selectionController.getAll("#productsContainer > .product");
        }

        if (text.length > 0) {

            searchResults.innerHTML = "";
            productsContainer.classList.add("hidden");

            var matchingIndexes = SearchIndex.match(text);
            var length = showAllResults ? matchingIndexes.length : Math.min(matchingIndexes.length, searchResultsLimit);

            if (!showAllResults && matchingIndexes.length > searchResultsLimit) {
                showAllSearchResults.classList.remove("hidden");
                resultsCount.textContent = matchingIndexes.length - searchResultsLimit;
            } else {
                showAllSearchResults.classList.add("hidden");
            }

            var clonedProductElements = [];

            for (var ii = 0; ii < length; ii++) {
                var clonedProductElement = productElements[matchingIndexes[ii]].cloneNode(true);
                clonedProductElements.push(clonedProductElement);
                searchResults.appendChild(clonedProductElement);
            }

            //Images.updateProductThumbnails(searchResults);

            searchResults.classList.remove("hidden");

        } else {
            showAllSearchResults.classList.add("hidden");
            searchResults.classList.add("hidden");
            productsContainer.classList.remove("hidden");
            // reset location and hash when we clear out search input
            location.hash = "";
            location.search = "";
        }
    }
    var init = function(pi, pdi, sc) {
        pIndex = pi;
        pdIndex = pdi;
        selectionController = sc;
        
        searchResults = selectionController.getById("searchResults");
        productsContainer = selectionController.getById("productsContainer");
        showAllResultsLink = selectionController.getById("showAllSearchResults");
        resultsCount = selectionController.getFromContainer(showAllResultsLink, ".resultsCount");

    }
    return {
        "search": search,
        "init": init
    }
} ();

var DLC = function() {
    var isDLC = function(pd) {
        return (pd &&
            pd.requiredProducts !== undefined &&
            pd.requiredProducts.length > 0);
    }
    return {
        "isDLC": isDLC
    }
} ();

var Owned = function() {
    var pdIndex;
    var gdIndex;
    var oIndex;
    var check = function(id) {

        if (owned === undefined) return false;

        var ownedProduct = oIndex.getElementByKey(id);
        if (ownedProduct) return true;

        // if all bundled products are owned then bundle is owned
        if (Bundles.isParent(id)) {
            var bundle = Bundles.getBundleByParentId(id);
            var childProductsOwned = true;
            for (var ii = 0; ii < bundle.children.length; ii++) {
                childProductsOwned &= check(bundle.children[ii]);
            }
            if (childProductsOwned) return true;
        }

        var productData = pdIndex && pdIndex.getElementByKey(id);

        // return false because if that is not DLC, we should have passed the check earlier
        if (!DLC.isDLC(productData)) return false;

        for (var rp = 0; rp < productData.requiredProducts.length; rp++) {
            var requiredProduct = productData.requiredProducts[rp].id;

            var gameDetails = gdIndex ? gdIndex.getElementByKey(requiredProduct) : undefined;

            if (!gameDetails ||
                !gameDetails.dlcs) continue;

            if (gameDetails.dlcs.length === 0) continue;

            for (var dd = 0; dd < gameDetails.dlcs.length; dd++) {
                var dlc = gameDetails.dlcs[dd];
                if (dlc.title === productData.title) return true;
            }
        }
    }
    var checkPartialOwnership = function(id) {
        if (Bundles.isParent(id) &&
            Bundles.checkChildrenAny(id, check)) return true;
    }
    var init = function(pdi, gdi, oi) {
        pdIndex = pdi;
        gdIndex = gdi;
        oIndex = oi;
    }
    return {
        "check": check,
        "checkPartialOwnership": checkPartialOwnership,
        "init": init
    }
} ();

var ProductClass = function() {
    var pdIndex, wIndex, uIndex;
    var getClass = function(product) {
        var productClass = [];

        if (Owned && Owned.check(product.id)) {
            productClass.push("owned");
        } else if (owned &&
            Owned.checkPartialOwnership &&
            Owned.checkPartialOwnership(product.id)) {
            productClass.push("partOwned");
        }

        if (wIndex && wIndex.check(product.id)) productClass.push("wishlisted");
        if (uIndex && uIndex.check(product.id)) productClass.push("updated");

        var pd = pdIndex && pdIndex.getElementByKey(product.id);
        if (pd && DLC.isDLC(pd)) productClass.push("dlc");

        if (Bundles.isParent(product.id)) productClass.push("bundle");

        if (ProductFiles.allFilesValidated(product.id)) productClass.push("validated");
        else if (ProductFiles.someFilesHaveValidationErrors(product.id)) productClass.push("validationError");

        if (pd &&
            pd.requiredProducts &&
            pd.requiredProducts.length &&
            Owned &&
            Owned.check(product.id)) {
            // if this is DLC - check validated status of the parent
            // because DLC downloads are extracted to the parent and 
            // validated as part of parent validation
            var requiredProductsValidated = true;
            for (var ii = 0; ii < pd.requiredProducts.length; ii++) {
                var rp = pd.requiredProducts[ii];
                requiredProductsValidated &= ProductFiles.allFilesValidated(rp.id);
            }
            if (requiredProductsValidated) productClass.push("validated");
        }

        return productClass;
    }
    var init = function(pdi, ui, wi) {
        pdIndex = pdi;
        wIndex = wi;
        uIndex = ui;
    }
    return {
        "init": init,
        "getClass": getClass
    }
} ();

var ViewModelProvider = function() {
    var pdIndex, gdIndex, wIndex, uIndex, sIndex, wikiIndex;
    var viewController;
    var getProductViewModel = function(product) {
        var productClass = ProductClass.getClass(product);
        return {
            "id": product.id,
            "productClass": productClass.join(" "),
            "productImage": product.image,
            "title": product.title
        };
    }
    var getRequiredProductViewModel = function(requiredProduct) {
        return requiredProduct;
    }
    var getDLCsViewModel = function(dlcs) {
        return {
            "dlcContent": dlcs.join(""),
            "dlcClass": dlcs.length ? "" : "hidden"
        }
    }
    var getBundleViewModel = function(bundles) {
        return {
            "bundleContent": bundles.join(""),
            "bundleClass": bundles.length ? "" : "hidden"
        }
    }
    var getTheme = function() {
        return document.body.classList.contains("dark") ?
            "dark" :
            "light";
    }
    var getFileViewModel = function(file) {
        if (file && !file.version) file.version = "";
        if (file && !file.validated) file.validatedClass = "hidden";
        return file;
    }
    var getScreenshotModel = function(screenshot) {
        return {
            "uri": Images.getRelativeUri(screenshot).screenshot
        }
    }
    var getSearchLinkModel = function(data) {
        return {
            "link": escape(data),
            "title": data
        }
    }
    var getTagModel = function(data) {
        return data;
    }
    var getOperatingSystemModel = function(worksOn, operatingSystem) {
        var os = "";
        if (operatingSystem === "Windows" && worksOn.Windows) {
            os = "Windows";
        } else if (operatingSystem === "Mac" && worksOn.Mac) {
            os = "Mac";
        } else if (operatingSystem === "Linux" && worksOn.Linux) {
            os = "Linux";
        }
        return {
            "operatingSystem": os,
            "theme": getTheme()
        }
    }
    var getWindowsModel = function(worksOn) {
        return getOperatingSystemModel(worksOn, "Windows");
    }
    var getMacModel = function(worksOn) {
        return getOperatingSystemModel(worksOn, "Mac");
    }
    var getLinuxModel = function(worksOn) {
        return getOperatingSystemModel(worksOn, "Linux");
    }
    var getGameDetailsViewModel = function(product) {

        if (!product) return;

        var pd = pdIndex && pdIndex.getElementByKey(product.id);
        var gd = gdIndex ? gdIndex.getElementByKey(product.id) : undefined;

        var productClass = ProductClass.getClass(product).join(" ");

        var openFolderLink, openFolderClass = "hidden";
        if (!Bundles.isParent(product.id) &&
            Owned.check(product.id) &&
            product.slug) {
            openFolderLink = product.slug;
            openFolderClass = "";
        }

        var images, productImageClass = "hidden";
        if (product.image) {
            images = Images.getRelativeUri(product.image);
            if (images) productImageClass = "";
        } else if (pd && pd.image) {
            images = Images.getRelativeUri(pd.image);
            if (images) productImageClass = "";
        }

        if (!images) {
            images = {};
        }

        var productTitle = product.title;

        var worksOnTitle, worksOnClass = "hidden";

        if (product.worksOn) {
            var worksOn = [];
            worksOn.push(viewController.createView(product.worksOn, getWindowsModel, "operatingSystemIcon"));
            worksOn.push(viewController.createView(product.worksOn, getMacModel, "operatingSystemIcon"));
            worksOn.push(viewController.createView(product.worksOn, getLinuxModel, "operatingSystemIcon"));

            worksOnTitle = worksOn.join(" ");
            if (worksOnTitle) worksOnClass = "";
        }

        var publisherTitle, publisherClass = "hidden";
        if (pd &&
            pd.publisher &&
            pd.publisher.name) {
            publisherTitle = viewController.createView(pd.publisher.name, getSearchLinkModel, "searchLink");
            if (publisherTitle) publisherClass = "";
        }

        var developerTitle, developerClass = "hidden";
        if (pd &&
            pd.developer &&
            pd.developer.name) {
            developerTitle = viewController.createView(pd.developer.name, getSearchLinkModel, "searchLink");
            if (developerTitle) developerClass = "";
        }

        var seriesTitle, seriesClass = "hidden";

        if (pd &&
            pd.series &&
            pd.series.name) {
            seriesTitle = viewController.createView(pd.series.name, getSearchLinkModel, "searchLink");
            if (seriesTitle) seriesClass = "";
        }

        var genresTitle, genresClass = "hidden";
        if (pd &&
            pd.genres &&
            pd.genres.length) {
            var genres = [];
            for (var ii = 0; ii < pd.genres.length; ii++) {
                genres.push(pd.genres[ii].name);
            }
            genresTitle = genres.join(", ");
            if (genresTitle) genresClass = "";
        }

        var cdKeyTitle, cdKeyClass = "hidden";
        var cdKeys = [];

        if (gd) {
            if (gd.cdKey) {
                cdKeys.push(gd.title + ": " + gd.cdKey);
            }

            if (gd.dlcs &&
                gd.dlcs.length) {
                for (var ii = 0; ii < gd.dlcs.length; ii++) {
                    if (gd.dlcs[ii].cdKey) {
                        cdKeys.push(gd.dlcs[ii].title + ": " + gd.dlcs[ii].cdKey);
                    }
                }
            }

            cdKeyTitle = cdKeys.join(", ");

            if (cdKeyTitle) cdKeyClass = "";
        }

        var requiredProductsTitle, requiredProductsClass = "hidden";
        if (pd &&
            pd.requiredProducts &&
            pd.requiredProducts.length) {
            var requiredProducts = [];
            for (var ii = 0; ii < pd.requiredProducts.length; ii++) {
                var requiredProductView = viewController.createView(pd.requiredProducts[ii], getRequiredProductViewModel, "requiredProduct");
                requiredProducts.push(requiredProductView);
            }
            requiredProductsTitle = requiredProducts.join(", ");
            if (requiredProductsTitle) requiredProductsClass = "";
        }

        // wikipedia

        var wikiLink, wikiClass = "hidden";
        var wiki = wikiIndex && wikiIndex.getElementByKey(product.id);
        if (wiki &&
            wiki.url) {
            wikiLink = wiki.url;
            wikiClass = "";
        }

        // files

        var files = ProductFiles.getFilesForProduct(product.id);
        var installersContent = "", extrasContent = "", extrasClass = "hidden", filesClass = "hidden";
        if (files &&
            files.length) {
            for (var ff = 0; ff < files.length; ff++) {
                if (files[ff].extra) {
                    var content = viewController.createView(files[ff], getFileViewModel, "fileExtra");
                    extrasContent += content;
                } else {
                    if (ProductFiles.hasBinExtension(files[ff].file)) continue;
                    var content = viewController.createView(files[ff], getFileViewModel, "fileInstaller");
                    installersContent += content;
                }
            }
            if (extrasContent) extrasClass = "";
            filesClass = "";
        }

        // changelog

        var changelogContent = "", changelogClass = "hidden";
        if (gd && gd.changelog) {
            changelogContent = gd.changelog;
            changelogClass = "";
        }

        // tags

        var tagsContent = "", tagsClass = "hidden";
        if (gd && gd.tags && gd.tags.length) {
            var tags = [];
            for (var ii = 0; ii < gd.tags.length; ii++) {
                tags.push(viewController.createView(gd.tags[ii], getTagModel, "tag"));
            }

            tagsContent = tags.join("");
            tagsClass = "";
        }

        // screenshots

        var productScreenshots = sIndex ? sIndex.getElementByKey(product.id) : undefined;
        var showScreenshotsClass = "hidden", screenshotsContent = "";
        var screenshotsCount = 0;

        if (productScreenshots &&
            productScreenshots.Value &&
            productScreenshots.Value.length) {
            screenshotsCount = productScreenshots.Value.length;

            for (var ss = 0; ss < productScreenshots.Value.length; ss++) {
                screenshotsContent += viewController.createView(productScreenshots.Value[ss], getScreenshotModel, "screenshot");
            }

            showScreenshotsClass = "";
        }


        return {
            "productTitle": productTitle,
            "productClass": productClass,
            "productImage": images.productImage,
            "productRetinaImage": images.productRetinaImage,
            "productImageClass": productImageClass,
            "worksOnTitle": worksOnTitle,
            "worksOnClass": worksOnClass,
            "developerTitle": developerTitle,
            "developerClass": developerClass,
            "publisherTitle": publisherTitle,
            "publisherClass": publisherClass,
            "seriesTitle": seriesTitle,
            "seriesClass": seriesClass,
            "cdKeyTitle": cdKeyTitle,
            "cdKeyClass": cdKeyClass,
            "genresTitle": genresTitle,
            "genresClass": genresClass,
            "requiredProductsTitle": requiredProductsTitle,
            "requiredProductsClass": requiredProductsClass,
            // files
            "filesClass": filesClass,
            "extrasClass": extrasClass,
            "installersContent": installersContent,
            "extrasContent": extrasContent,
            // screenshots
            "showScreenshotsClass": showScreenshotsClass,
            "screenshotsCount": screenshotsCount,
            "screenshotsContent": screenshotsContent,
            // changelogContainer
            "changelogContent": changelogContent,
            "changelogClass": changelogClass,
            // tagsClass
            "tagsClass": tagsClass,
            "tagsContent": tagsContent,
            // wiki
            "wikiLink": wikiLink,
            "wikiClass": wikiClass
        }
    }
    var init = function(pdi, gdi, ui, wi, si, wikii, vc) {
        pdIndex = pdi;
        gdIndex = gdi;
        wIndex = wi;
        uIndex = ui;
        sIndex = si;
        wikiIndex = wikii;
        viewController = vc;
    }
    return {
        "init": init,
        "getProductViewModel": getProductViewModel,
        "getGameDetailsViewModel": getGameDetailsViewModel,
        "getDLCsViewModel": getDLCsViewModel,
        "getBundleViewModel": getBundleViewModel
    }
} ();

var Bundles = function() {
    var parents = [];
    var children = [];
    var init = function() {
        if (!bundles || !bundles.length) return;
        for (var ii = 0; ii < bundles.length; ii++) {
            parents.push(bundles[ii].parent);
            for (var jj = 0; jj < bundles[ii].children.length; jj++) {
                children.push(bundles[ii].children[jj]);
            }
        }
    }
    var isParent = function(id) {
        return parents.indexOf(id) > -1;
    }
    var isChild = function(id) {
        return children.indexOf(id) > -1;
    }
    var getBundleByParentId = function(id) {
        for (var ii = 0; ii < bundles.length; ii++) {
            if (bundles[ii].parent === id) {
                return bundles[ii];
            }
        }
    }
    var checkChildrenAny = function(id, predicate) {
        return checkChildrenInternal(id, predicate, false, function(a, b) {
            return a | b;
        });
    }
    var checkChildrenAll = function(id, predicate) {
        return checkChildrenInternal(id, predicate, true, function(a, b) {
            return a & b;
        });
    }
    var checkChildrenInternal = function(id, predicate, initialValue, comparison) {
        var bundle = getBundleByParentId(id);
        var result = initialValue;
        for (var ii = 0; ii < bundle.children.length; ii++) {
            result = comparison(result, predicate(bundle.children[ii]));
        }
        return result;
    }
    return {
        "init": init,
        "isParent": isParent,
        "isChild": isChild,
        "getBundleByParentId": getBundleByParentId,
        "checkChildrenAny": checkChildrenAny,
        "checkChildrenAll": checkChildrenAll
    }
} ();

var ProductFiles = function() {
    var binExtension = ".bin";
    var getFilesForProduct = function(id) {
        if (!productfiles ||
            id === 0) return;
        var files = [];
        for (var ii = 0; ii < productfiles.length; ii++)
            if (productfiles[ii].id === id) files.push(productfiles[ii]);
        return files;
    }
    var hasBinExtension = function(filename) {
        return filename && filename.indexOf(binExtension) === filename.length - binExtension.length;
    }
    var allFilesValidated = function(id) {
        var files = getFilesForProduct(id);
        if (!files) return false;
        var validated = files && files.length > 0;
        for (var ii = 0; ii < files.length; ii++)
            validated &= files[ii].validated;
        return validated;
    }
    var someFilesHaveValidationErrors = function (id) {
        var files = getFilesForProduct(id);
        if (!files) return false;
        for (var ii = 0; ii < files.length; ii++)
            if (files[ii].validated === false) return true;
        return false;
    }
    return {
        "getFilesForProduct": getFilesForProduct,
        "hasBinExtension": hasBinExtension,
        "allFilesValidated": allFilesValidated,
        "someFilesHaveValidationErrors": someFilesHaveValidationErrors
    }
} ();

var NavigationController = function() {
    var selectionController;
    var productsContainer;
    var searchResults;
    var searchContainer;
    var init = function(sc) {
        selectionController = sc;
        
        productsContainer = selectionController.getById("productsContainer");
        searchResults = selectionController.getById("searchResults");
        searchContainer = selectionController.getById("searchContainer");
    }
    var update = function() {
        if (location.hash.indexOf("#") > -1) {
            if (location.hash === "#showAll") {
                GameDetails.close();
                productsContainer.classList.add("hidden");
                searchResults.classList.remove("hidden");
                searchContainer.classList.remove("hidden");
                return;
            }
            var productId = parseInt(location.hash.substr(1));
            if (productId !== undefined) GameDetails.show(productId);
            productsContainer.classList.add("hidden");
            searchResults.classList.add("hidden");
            searchContainer.classList.add("hidden");
        } else {
            GameDetails.close();
            productsContainer.classList.remove("hidden");
            searchResults.classList.remove("hidden");
            searchContainer.classList.remove("hidden");
        }
    }
    return {
        "update": update,
        "init": init
    }
} ();

var Screenshots = function() {
    var show = function(element) {
        if (element &&
            element.parentNode) {
            var screenshotsContent = _$(element.parentNode, ".screenshots");
            var images = _$$(screenshotsContent, "img");
            for (var ii = 0; ii < images.length; ii++) {
                var image = images[ii];
                image.src = image.getAttribute("data-src");
            }
            screenshotsContent.classList.toggle("hidden");
        }
    }
    return {
        "show": show
    }
} ();

document.addEventListener("DOMContentLoaded", function() {

    if (products === undefined) {
        $("errorMessage").classList.remove("hidden");
        $("searchContainer").classList.add("hidden");
        $("toggleMenu").classList.add("hidden");
        return;
    }

    var getId = function(e) {
        return e.id
    };
    var getValue = function(e) {
        return e;
    };
    var getKey = function(e) {
        return e.Key;
    }

    var start = new Date();
    var cp = [];
    var addedProducts = [];

    // first add available products...
    for (var ii = 0; ii < products.length; ii++) {
        cp.push(products[ii]);
        addedProducts.push(products[ii].id);
    }
    // ...then add owned products that are no longer available
    if (owned !== undefined) {
        for (var ii = 0; ii < owned.length; ii++) {
            if (addedProducts.indexOf(owned[ii].id) > -1) continue;
            cp.push(owned[ii]);
        }
    }

    var elapsed = new Date() - start;
    console.log("Combining products: " + elapsed + "ms");

    start = new Date();
    
    var selectionController = new SelectionController();
    var templateController = new TemplateController(selectionController);
    var viewController = new ViewController(templateController);
    
    var informationController = new InformationController(selectionController);
    var changelogController = new ChangelogController(selectionController);
    var mnemonicsController = new MnemonicsController(selectionController);    
    
    var productsIndex = IndexedCollection.create(cp, getId);
    var productsDataIndex = productsdata ? IndexedCollection.create(productsdata, getId) : undefined;
    var gameDetailsIndex = gamedetails ? IndexedCollection.create(gamedetails, getId) : undefined;
    var ownedIndex = owned ? IndexedCollection.create(owned, getId) : undefined;
    var updatedIndex = updated ? IndexedCollection.create(updated, getValue) : undefined;
    var wishlistedIndex = wishlisted ? IndexedCollection.create(wishlisted, getValue) : undefined;
    var screenshotsIndex = screenshots ? IndexedCollection.create(screenshots, getKey) : undefined;
    var wikiIndex = wikipedia ? IndexedCollection.create(wikipedia, getId) : undefined;

    ProductClass.init(productsDataIndex, updatedIndex, wishlistedIndex);
    ViewModelProvider.init(productsDataIndex, gameDetailsIndex, updatedIndex, wishlistedIndex, screenshotsIndex, wikiIndex, viewController);
    Owned.init(productsDataIndex, gameDetailsIndex, ownedIndex);
    GameDetails.init(productsIndex, productsDataIndex, gameDetailsIndex, selectionController, viewController);

    elapsed = new Date() - start;
    console.log("Indexing collections: " + elapsed + "ms");

    start = new Date();
    Bundles.init();
    elapsed = new Date() - start;
    console.log("Initializing bundles: " + elapsed + "ms");

    start = new Date();
    var html = [];
    for (var ii = 0; ii < productsIndex.getLength(); ii++) {
        var product = productsIndex.getElementByIndex(ii);

        if (Bundles.isChild(product.id)) continue;

        var view = viewController.createView(product, ViewModelProvider.getProductViewModel, "product");
        html.push(view);
    }
    elapsed = new Date() - start;
    console.log("Creating views: " + elapsed + "ms");

    var start = new Date();

    var pContainer = document.getElementById("productsContainer");

    requestAnimationFrame(() => {
        pContainer.innerHTML = html.join("");
        Images.updateProductThumbnails(pContainer);
        informationController.initialize();

        requestAnimationFrame(() => {
            var loadingScreen = selectionController.getById("loadingScreen");
            loadingScreen.classList.add("hidden");
        });

    });

    var elapsed = new Date() - start;
    console.log("Adding html to the DOM: " + elapsed + "ms");

    start = new Date();
    SearchIndex.init(
        productsIndex,
        productsDataIndex,
        gameDetailsIndex,
        ownedIndex,
        updatedIndex,
        wishlistedIndex);
    elapsed = new Date() - start;
    console.log("Initializing search index: " + elapsed + "ms");

    Search.init(productsIndex, productsDataIndex, selectionController);
    var searchInput = selectionController.getById("searchInput");
    searchInput.addEventListener("input", function(e) {
        Search.search(searchInput.value.toLowerCase())
    });

    var showAllSearchResults = selectionController.getById("showAllSearchResults");
    showAllSearchResults.addEventListener("click", function() {
        Search.search(searchInput.value.toLowerCase(), true);
    });

    var toggleInfo = selectionController.getById("toggleInfo");
    toggleInfo.addEventListener("click", function(e) {
        var infoContainer = selectionController.getById("infoContainer");
        infoContainer.classList.toggle("hidden");
    });

    var backToTop = selectionController.getById("backToTop");
    backToTop.addEventListener("click", function() {
        document.body.scrollTop = 0;
    });

    var switchTheme = selectionController.getById("switchTheme");
    switchTheme.addEventListener("click", function(e) {
        document.body.classList.toggle("dark");
        document.body.classList.toggle("light");
    });

    window.addEventListener("hashchange", NavigationController.update);

    NavigationController.init(selectionController);
    NavigationController.update();

    var menuContainer = selectionController.getById("menuContainer");
    var toggleMenu = selectionController.getById("toggleMenu");
    toggleMenu.addEventListener("click", function(e) {
        menuContainer.classList.toggle("hidden");
    });

    // check if we're navigating to search
    if (location.search) {
        requestAnimationFrame(function() {
            var searchString = unescape(location.search.substr(1, location.search.length - 1));
            searchInput.value = searchString;
            Search.search(searchString.toLowerCase(), true);
        });
    } else if (location.hash) {
        requestAnimationFrame(NavigationController.update);
    }

    mnemonicsController.initializeForContainer(document.body);


});