package cmd

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/vangogh/cmd/selectors"
	"github.com/boggydigital/vangogh/internal"
	"strconv"
	"time"
)

func Route(req *clo.Request, defs *clo.Definitions) error {
	if req == nil {
		return clo.Route(nil, defs)
	}

	verbose := req.Flag("verbose")

	productType := req.ArgVal("product-type")
	media := req.ArgVal("media")

	pt := vangogh_products.Parse(productType)
	mt := gog_media.Parse(media)

	missing := req.Flag("missing")
	all := req.Flag("all")

	idSet, err := selectors.StrSetFrom(selectors.Id{
		Ids:       req.ArgValues("id"),
		Slugs:     req.ArgValues("slug"),
		FromStdin: req.Flag("read-ids"),
	})
	if err != nil {
		return err
	}

	osStrings := req.ArgValues("operating-system")
	langCodes := req.ArgValues("language-code")
	dtStrings := req.ArgValues("download-type")
	operatingSystems := vangogh_downloads.ParseManyOperatingSystems(osStrings)
	downloadTypes := vangogh_downloads.ParseManyDownloadTypes(dtStrings)

	switch req.Command {
	case "auth":
		username := req.ArgVal("username")
		password := req.ArgVal("password")
		return Authenticate(username, password)
	case "cleanup":
		return Cleanup(idSet, mt, operatingSystems, langCodes, downloadTypes, all)
	case "digest":
		property := req.ArgVal("property")
		return Digest(property)
	case "extract":
		properties := req.ArgValues("properties")
		return Extract(0, mt, properties)
	case "get-data":
		updated := req.Flag("updated")
		since := time.Now().Unix()
		if updated {
			since = time.Now().Add(-time.Hour * 24).Unix()
		}
		denyIdsFile := req.ArgVal("deny-ids-file")
		denyIds := internal.ReadLines(denyIdsFile)
		return GetData(idSet, denyIds, pt, mt, since, missing, updated, verbose)
	case "get-downloads":
		update := req.Flag("update")
		mha, err := hoursAtoi(req.ArgVal("modified-hours-ago"))
		if err != nil {
			return err
		}
		var modifiedSince int64 = 0
		if mha > 0 {
			modifiedSince = time.Now().Add(-time.Hour * time.Duration(mha)).Unix()
		}
		forceRemoteUpdate := req.Flag("force-remote-update")
		validate := req.Flag("validate")
		noCleanup := req.Flag("no-cleanup")
		return GetDownloads(
			idSet,
			mt,
			operatingSystems,
			downloadTypes,
			langCodes,
			missing,
			update,
			modifiedSince,
			forceRemoteUpdate,
			validate,
			noCleanup)
	case "get-images":
		imageTypes := req.ArgValues("image-type")
		its := make([]vangogh_images.ImageType, 0, len(imageTypes))
		for _, imageType := range imageTypes {
			its = append(its, vangogh_images.Parse(imageType))
		}
		return GetImages(idSet, its, missing)
	case "get-videos":
		return GetVideos(idSet, missing)
	case "info":
		allText := req.Flag("all-text")
		images := req.Flag("images")
		videoId := req.Flag("video-id")
		return Info(idSet, allText, images, videoId)
	case "list":
		mha, err := hoursAtoi(req.ArgVal("modified-hours-ago"))
		if err != nil {
			return err
		}
		var modifiedSince int64 = 0
		if mha > 0 {
			modifiedSince = time.Now().Add(-time.Hour * time.Duration(mha)).Unix()
		}
		properties := req.ArgValues("property")
		return List(idSet, modifiedSince, pt, mt, properties)
	case "owned":
		return Owned(idSet)
	case "search":
		query := make(map[string][]string)
		for _, prop := range vangogh_properties.Searchable() {
			if values, ok := req.Arguments[prop]; ok && len(values) > 0 {
				query[prop] = values
			}
		}
		return Search(query)
	case "size":
		return Size(idSet, mt, operatingSystems, downloadTypes, langCodes, missing)
	case "scrub-data":
		fix := req.Flag("fix")
		return ScrubData(mt, fix)
	case "summary":
		sha, err := hoursAtoi(req.ArgVal("since-hours-ago"))
		if err != nil {
			return err
		}
		since := time.Now().Unix() - int64(sha*60*60)
		return Summary(since, mt)
	case "sync":
		data := req.Flag("data")
		images := req.Flag("images")
		screenshots := req.Flag("screenshots")
		videos := req.Flag("videos")
		downloadsUpdates := req.Flag("downloads-updates")
		missingDownloads := req.Flag("missing-downloads")
		if all {
			data, images, screenshots, videos, downloadsUpdates = true, true, true, true, true
		}
		data = data && !req.Flag("no-data")
		images = images && !req.Flag("no-images")
		screenshots = screenshots && !req.Flag("no-screenshots")
		videos = videos && !req.Flag("no-videos")
		downloadsUpdates = downloadsUpdates && !req.Flag("no-downloads-updates")
		sha, err := hoursAtoi(req.ArgVal("since-hours-ago"))
		if err != nil {
			return err
		}
		return Sync(
			mt,
			sha,
			data, images, screenshots, videos, downloadsUpdates, missingDownloads,
			operatingSystems,
			downloadTypes,
			langCodes,
			verbose)
	case "tag":
		operation := req.ArgVal("operation")
		tagName := req.ArgVal("tag-name")
		return Tag(idSet, operation, tagName)
	case "validate":
		return Validate(idSet, mt, operatingSystems, langCodes, downloadTypes, all)
	case "wishlist":
		addProductIds := req.ArgValues("add")
		removeProductIds := req.ArgValues("remove")
		return Wishlist(mt, addProductIds, removeProductIds)
	default:
		return clo.Route(req, defs)
	}
}

func hoursAtoi(str string) (int, error) {
	var sha int
	var err error
	if str != "" {
		sha, err = strconv.Atoi(str)
		if err != nil {
			return 0, err
		}
	}
	return sha, nil
}
