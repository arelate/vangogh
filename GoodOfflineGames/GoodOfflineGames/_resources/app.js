"use strict";

var $ = function(id) { return document.getElementById(id); }
var _$ = function(n, s) { return n.querySelector(s); }
var $$ = function(s) { return document.querySelectorAll(s); }
var _$$ = function(n, s) { return n.querySelectorAll(s); }

var Templates = function() {
    var getProductTemplate = function() {
        return "<div class='product {{productClass}}' title='{{title}}'><a href='#{{id}}'>" +
            "<div class='productImageContainer'><img class='hidden' data-src='{{productImage}}' /></div>" +
            "<span class='title' >{{title}}</span></a></div>";
    }
    var gameDetailsImageTemplate = "<img class='image {{productImageClass}}' srcset='{{productRetinaImage}} 2x, {{productImage}} 1x' src='{{productImage}}' onerror='Images.hideOnError(this)' />";
    var gameDetailsProductHeader = "<div class='productTitle header1 {{productClass}}'>{{productTitle}}</div>";
    var gameDetailsDescription = "<table border='0' cellspacing='0' cellpadding='0' class='descriptionContainer'>" +
        "<tr class='{{developerClass}}'><td class='header'>Developer</td><td class='title'>{{developerTitle}}</td></tr>" +
        "<tr class='{{publisherClass}}'><td class='header'>Publisher</td><td class='title'>{{publisherTitle}}</td></tr>" +
        "<tr class='{{genresClass}}'><td class='header'>Genres</td><td class='title'>{{genresTitle}}</td></tr>" +
        "<tr class='{{seriesClass}}'><td class='header'>Series</td><td class='title'>{{seriesTitle}}</td></tr>" +
        "<tr class='{{cdKeyClass}}'><td class='header'>CD Key</td><td class='title cdKey' title='Select to reveal'>{{cdKeyTitle}}</td></tr>" +
        "<tr class='{{requiredProductsClass}}'><td class='header'>Required products</td><td class='title'>{{requiredProductsTitle}}</td></tr>" +
        "<tr class='{{worksOnClass}}'><td class='header'>Works on</td><td class='title worksOn'>{{worksOnTitle}}</td></tr>" +
        "<tr class='{{wikiClass}}'><td class='header'>Wikipedia</td><td class='title'><a href={{wikiLink}} accesskey='w'>Article</a></td></tr>" +
        "<tr class='{{tagsClass}}'><td class='header'>Tags</td><td class='title'>{{tagsContent}}</td></tr>" +
        "</table>";
    var gameDetailsFileContainer = "<div class='filesContainer {{filesClass}}'>" +
        "<h1>Installers, patches</h1><div>{{installersContent}}</div>" +
        "<div class='{{extrasClass}}'><h1>Extras</h1><div>{{extrasContent}}</div></div>";
    var gameDetailsScreenshotsContainer = "<div class='screenshotsContainer'>" +
        "<button accesskey='s' class='{{showScreenshotsClass}} showHideButton' onclick='Screenshots.show(this);'>Show/hide {{screenshotsCount}} screenshots</button>" +
        "<div class='screenshots hidden'>{{screenshotsContent}}</div>" +
        "</div>";
    var gameDetailsChangelogContainer = "<div id='changelogContainer' class='{{changelogClass}}'>" +
        "<button accesskey='c'class='showHideButton' onclick='Changelog.show(this);'>Show/hide changelog</button>" +
        "<div class='changelogContent hidden'>{{changelogContent}}</div>" +
        "</div>";
    var getGameDetailsTemplate = function() {
        return "<div class='productContainer'>" +
            gameDetailsProductHeader +
            gameDetailsImageTemplate +
            gameDetailsDescription +
            gameDetailsScreenshotsContainer +
            gameDetailsChangelogContainer +
            gameDetailsFileContainer +
            "</div>";
    }
    var getDLCsTemplate = function() {
        return "<div class='dlcTitle header1 {{dlcClass}}'>Downloadable content</div>" +
            "<div class='dlcContainer {{dlcClass}}'>{{dlcContent}}</div>";
    }
    var getBundleTemplate = function() {
        return "<div class='bundleTitle header1 {{bundleClass}}'>Bundled products</div>" +
            "<div class='bundleContainer {{bundleClass}}'>{{bundleContent}}</div>";
    }
    var getRequiredProductTemplate = function() {
        return "<a href='#{{id}}'>{{title}}</a>"
    }
    var getFileInstallerTemplate = function() {
        return "<span class='file entry'><a href='./{{folder}}/{{file}}'>" +
            getOperatingSystemIconTemplate() +
            "<span class='linkText'>{{name}}, {{version}} {{size}}</span></a></span>";
    }
    var getFileExtraTemplate = function() {
        return "<span class='extra entry'><a href='./{{folder}}/{{file}}'>" +
            getExtraIconTemplate() +
            "<span class='linkText'>{{name}} {{size}}</span></a></span>";
    }
    var getScreenshotTemplate = function() {
        return "<a href='{{uri}}'><img data-src={{uri}} onerror='Images.hideOnError(this)'/></a>";
    }
    var getOperatingSystemIconTemplate = function() {
        return "<i class='icon fa-{{operatingSystem}}' title='{{operatingSystem}}'></i>";
    }
    var getExtraIconTemplate = function() {
        return "<i class='icon fa-star' title='Extra'></i>";
    }
    var getSearchLinkTemplate = function() {
        return "<a href='?{{link}}'>{{title}}</a>";
    }
    var getTagTemplate = function() {
        return "<span class='tag'>{{name}}</span>";
    }
    return {
        "getProductTemplate": getProductTemplate,
        "getGameDetailsTemplate": getGameDetailsTemplate,
        "getRequiredProductTemplate": getRequiredProductTemplate,
        "getDLCsTemplate": getDLCsTemplate,
        "getBundleTemplate": getBundleTemplate,
        "getFileInstallerTemplate": getFileInstallerTemplate,
        "getFileExtraTemplate": getFileExtraTemplate,
        "getOperatingSystemIconTemplate": getOperatingSystemIconTemplate,
        "getScreenshotTemplate": getScreenshotTemplate,
        "getSearchLinkTemplate": getSearchLinkTemplate,
        "getTagTemplate": getTagTemplate
    }
} ();

var Views = function() {
    var bindTemplateToModel = function(template, model) {
        var result = template;
        for (var property in model) {
            var replacedProperty = "{{" + property + "}}";
            while (result.indexOf(replacedProperty) > -1)
                result = result.replace(replacedProperty, model[property]);
        }
        return result;
    }
    var createView = function(model, viewModelProvider, templateProvider) {
        var viewModel = viewModelProvider(model);
        var template = templateProvider();
        return viewModel !== undefined ? bindTemplateToModel(template, viewModel) : "";
    }
    return {
        "createView": createView
    }
} ();

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
            thumbnails.push(productThumbnails[ii]);
        }
        updateProductThumbnail();
    }
    var updateProductThumbnail = function() {
        if (!thumbnails || !thumbnails.length) return;
        for (var ii = 0; ii < 10; ii++) {
            var image = thumbnails.shift();
            if (!image) break;
            var source = image.getAttribute("data-src");
            image.srcset = getThumbnailSrc(source);
            image.removeAttribute("data-src");
            image.classList.remove("hidden");
        }
        requestAnimationFrame(updateProductThumbnail);
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
    var gameDetailsContainer = $("gameDetailsContainer");
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
            dlcContent.push(Views.createView(dlcProducts[ii], ViewModelProvider.getGameDetailsViewModel, Templates.getGameDetailsTemplate));

        return Views.createView(dlcContent, ViewModelProvider.getDLCsViewModel, Templates.getDLCsTemplate);
    }
    var show = function(id) {
        var product = pIndex.getElementByKey(id);
        if (!product) return;

        var markup = "";

        var htmlView = Views.createView(product, ViewModelProvider.getGameDetailsViewModel, Templates.getGameDetailsTemplate);

        markup = htmlView;
        markup += showDLC(id);

        // show bundled products
        if (Bundles.isParent(id)) {
            var bundleContent = [],
                bundleClass;

            var bundle = Bundles.getBundleByParentId(id);
            for (var ii = 0; ii < bundle.children.length; ii++) {
                var product = pIndex.getElementByKey(bundle.children[ii]);

                var productView = Views.createView(product, ViewModelProvider.getGameDetailsViewModel, Templates.getGameDetailsTemplate);
                var dlcView = showDLC(bundle.children[ii]);

                bundleContent.push(productView + dlcView);
            }

            var bundleView = Views.createView(bundleContent, ViewModelProvider.getBundleViewModel, Templates.getBundleTemplate);

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
    var init = function(pi, pdi, gdi) {
        pIndex = pi;
        pdIndex = pdi;
        gdIndex = gdi;
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
    var searchResults = $("searchResults");
    var productsContainer = $("productsContainer");
    var showAllResultsLink = $("showAllSearchResults");
    var resultsCount = _$(showAllResultsLink, ".resultsCount");
    var searchResultsLimit = 25;
    var search = function(text, showAllResults) {

        if (productElements === undefined) {
            productElements = $$("#productsContainer > .product");
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

            Images.updateProductThumbnails(searchResults);

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
    var init = function(pi, pdi) {
        pIndex = pi;
        pdIndex = pdi;
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
        if (file && !file.theme) file.theme = getTheme();
        if (file && !file.version) file.version = "";
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
            worksOn.push(Views.createView(product.worksOn, getWindowsModel, Templates.getOperatingSystemIconTemplate));
            worksOn.push(Views.createView(product.worksOn, getMacModel, Templates.getOperatingSystemIconTemplate));
            worksOn.push(Views.createView(product.worksOn, getLinuxModel, Templates.getOperatingSystemIconTemplate));

            worksOnTitle = worksOn.join(" ");
            if (worksOnTitle) worksOnClass = "";
        }

        var publisherTitle, publisherClass = "hidden";
        if (pd &&
            pd.publisher &&
            pd.publisher.name) {
            publisherTitle = Views.createView(pd.publisher.name, getSearchLinkModel, Templates.getSearchLinkTemplate);
            if (publisherTitle) publisherClass = "";
        }

        var developerTitle, developerClass = "hidden";
        if (pd &&
            pd.developer &&
            pd.developer.name) {
            developerTitle = Views.createView(pd.developer.name, getSearchLinkModel, Templates.getSearchLinkTemplate);
            if (developerTitle) developerClass = "";
        }

        var seriesTitle, seriesClass = "hidden";

        if (pd &&
            pd.series &&
            pd.series.name) {
            seriesTitle = Views.createView(pd.series.name, getSearchLinkModel, Templates.getSearchLinkTemplate);
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
                var requiredProductView = Views.createView(pd.requiredProducts[ii], getRequiredProductViewModel, Templates.getRequiredProductTemplate);
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
                    var content = Views.createView(files[ff], getFileViewModel, Templates.getFileExtraTemplate);
                    extrasContent += content;
                } else {
                    if (ProductFiles.hasBinExtension(files[ff].file)) continue;
                    var content = Views.createView(files[ff], getFileViewModel, Templates.getFileInstallerTemplate);
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
                tags.push(Views.createView(gd.tags[ii], getTagModel, Templates.getTagTemplate));
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
                screenshotsContent += Views.createView(productScreenshots.Value[ss], getScreenshotModel, Templates.getScreenshotTemplate);
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
    var init = function(pdi, gdi, ui, wi, si, wikii) {
        pdIndex = pdi;
        gdIndex = gdi;
        wIndex = wi;
        uIndex = ui;
        sIndex = si;
        wikiIndex = wikii;
    }
    return {
        "init": init,
        "getProductViewModel": getProductViewModel,
        "getGameDetailsViewModel": getGameDetailsViewModel,
        "getDLCsViewModel": getDLCsViewModel,
        "getBundleViewModel": getBundleViewModel
    }
} ();

var IndexedCollection = function() {
    var indexKeys = function(collection, getElementKey) {
        var index = [];
        collection.forEach(function(i) {
            index[getElementKey(i)] = collection.indexOf(i);
        });
        return index;
    }
    var create = function(collection, getElementKey) {
        var index = indexKeys(collection, getElementKey);
        var getElementByKey = function(key) {
            return collection[index[key]];
        }
        var getElementByIndex = function(itemIndex) {
            return collection[itemIndex];
        }
        var check = function(id) {
            if (Bundles &&
                Bundles.isParent &&
                Bundles.isParent(id) &&
                Bundles.checkChildrenAny &&
                Bundles.checkChildrenAny(id, check)) return true;

            return getElementByKey(id) !== undefined;
        }
        return {
            "getElementByKey": getElementByKey,
            "getElementByIndex": getElementByIndex,
            "check": check,
            "getLength": function() {
                return collection.length
            }
        }
    }
    return {
        "create": create
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

var Info = function() {
    var infoContainer = $("infoContainer");
    var init = function() {
        var classes = [
            "product",
            "product owned",
            "product owned updated",
            "product wishlisted",
            "product bundle",
            "product bundle partOwned",
            "product bundle partOwned updated",
            "product bundle owned",
            "product bundle owned updated",
            "product bundle wishlisted",
            "product dlc",
            "product dlc owned",
            "product dlc wishlisted"
        ];
        var titles = [
            "Avail. products",
            "Owned products",
            "Upd. products",
            "Wishlisted products",
            "Bundles",
            "Part. owned bundles",
            "Upd. part. owned bundles",
            "Owned bundles",
            "Upd. bundles",
            "Wishlisted bundles",
            "DLCs",
            "Owned DLCs",
            "Wishlisted DLCs"
        ];

        if (classes.length !== titles.length) {
            alert("Inconsistent information data!");
            return;
        }

        for (var i = 0; i < classes.length; i++) {
            var count = document.getElementsByClassName(classes[i]).length;
            var infoEntity = document.createElement("div");
            infoEntity.className = "info";

            var entityClass = document.createElement("span");
            entityClass.className = classes[i];
            entityClass.textContent = titles[i];
            infoEntity.appendChild(entityClass);

            var entityCount = document.createElement("span");
            entityCount.className = "product count";
            entityCount.textContent = count;
            infoEntity.appendChild(entityCount);

            infoContainer.appendChild(infoEntity);
        }
    }
    return {
        "init": init
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
    return {
        "getFilesForProduct": getFilesForProduct,
        "hasBinExtension": hasBinExtension
    }
} ();

var NavigationController = function() {
    var productsContainer = $("productsContainer");
    var searchResults = $("searchResults");
    var searchContainer = $("searchContainer");
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
        "update": update
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

var Changelog = function() {
    var show = function(element) {
        if (element &&
            element.parentNode) {
            var changelogContent = _$(element.parentNode, ".changelogContent");
            changelogContent.classList.toggle("hidden");
        }
    }
    return {
        "show": show
    }
} ();

var Mnemonics = function() {
    var mnemonicsVisible = false;
    var init = function(container) {

        var accessKeyElements = _$$(container, "*[accessKey]");
        for (var ii = 0; ii < accessKeyElements.length; ii++) {
            var mnemonic = document.createElement("div");
            mnemonic.className = "mnemonic hidden";
            mnemonic.textContent = accessKeyElements[ii].accessKey;
            accessKeyElements[ii].appendChild(mnemonic);
        }


        window.addEventListener("keydown", function(e) {
            if (!e.altKey || mnemonicsVisible) return;

            mnemonicsVisible = true;

            var mnemonics = $$(".mnemonic");
            for (var ii = 0; ii < mnemonics.length; ii++) {
                mnemonics[ii].classList.remove("hidden");
            }

        });

        window.addEventListener("keyup", function(e) {
            if (e.keyCode !== 18) return;

            mnemonicsVisible = false;

            var mnemonics = $$(".mnemonic");
            for (var ii = 0; ii < mnemonics.length; ii++) {
                mnemonics[ii].classList.add("hidden");
            }
        });
    }
    return {
        "init": init
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
    var productsIndex = IndexedCollection.create(cp, getId);
    var productsDataIndex = productsdata ? IndexedCollection.create(productsdata, getId) : undefined;
    var gameDetailsIndex = gamedetails ? IndexedCollection.create(gamedetails, getId) : undefined;
    var ownedIndex = owned ? IndexedCollection.create(owned, getId) : undefined;
    var updatedIndex = updated ? IndexedCollection.create(updated, getValue) : undefined;
    var wishlistedIndex = wishlisted ? IndexedCollection.create(wishlisted, getValue) : undefined;
    var screenshotsIndex = screenshots ? IndexedCollection.create(screenshots, getKey) : undefined;
    var wikiIndex = wikipedia ? IndexedCollection.create(wikipedia, getId) : undefined;

    ProductClass.init(productsDataIndex, updatedIndex, wishlistedIndex);
    ViewModelProvider.init(productsDataIndex, gameDetailsIndex, updatedIndex, wishlistedIndex, screenshotsIndex, wikiIndex);
    Owned.init(productsDataIndex, gameDetailsIndex, ownedIndex);
    GameDetails.init(productsIndex, productsDataIndex, gameDetailsIndex);

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

        var view = Views.createView(product, ViewModelProvider.getProductViewModel, Templates.getProductTemplate);
        html.push(view);
    }
    elapsed = new Date() - start;
    console.log("Creating views: " + elapsed + "ms");

    var start = new Date();
    // show first N elements ASAP, others delay for a bit
    var throttle = Math.min(150, html.length);
    var initialHtml = html.slice(0, throttle).join("");
    var pContainer = document.getElementById("productsContainer");
    pContainer.innerHTML = initialHtml;

    var remainingHtml = html.slice(throttle, html.length).join("");
    requestAnimationFrame(function() {
        pContainer.innerHTML += remainingHtml;
        if (!location.search && !location.hash) pContainer.classList.remove("hidden");
        Info.init();
        
        Images.updateProductThumbnails(pContainer);
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

    Search.init(productsIndex, productsDataIndex);
    var searchInput = $("searchInput");
    searchInput.addEventListener("input", function(e) {
        Search.search(searchInput.value.toLowerCase())
    });

    var showAllSearchResults = $("showAllSearchResults");
    showAllSearchResults.addEventListener("click", function() {
        Search.search(searchInput.value.toLowerCase(), true);
    });

    var toggleInfo = $("toggleInfo");
    toggleInfo.addEventListener("click", function(e) {
        var infoContainer = $("infoContainer");
        infoContainer.classList.toggle("hidden");
    });

    var backToTop = $("backToTop");
    backToTop.addEventListener("click", function() {
        document.body.scrollTop = 0;
    });

    var switchTheme = $("switchTheme");
    switchTheme.addEventListener("click", function(e) {
        document.body.classList.toggle("dark");
        document.body.classList.toggle("light");
    });

    window.addEventListener("hashchange", NavigationController.update);

    NavigationController.update();

    var menuContainer = $("menuContainer");
    var toggleMenu = $("toggleMenu");
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

    Mnemonics.init(document.body);


});