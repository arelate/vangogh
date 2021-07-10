package cmd

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/clo"
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

	ids := req.ArgValues("id")
	if req.Flag("read-ids") {
		var err error
		ids, err = internal.ReadStdinIds()
		if err != nil {
			return err
		}
	}

	slug := req.ArgVal("slug")
	missing := req.Flag("missing")

	switch req.Command {
	case "auth":
		username := req.ArgVal("username")
		password := req.ArgVal("password")
		return Authenticate(username, password)
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
		return GetData(ids, slug, denyIds, pt, mt, since, missing, updated, verbose)
	case "get-downloads":
		osStrings := req.ArgValues("operating-system")
		langCodes := req.ArgValues("language-code")
		dtStrings := req.ArgValues("download-type")
		return GetDownloads(ids, slug, mt, osStrings, langCodes, dtStrings, missing)
	case "get-images":
		imageType := req.ArgVal("image-type")
		it := vangogh_images.Parse(imageType)
		return GetImages(ids, slug, it, nil, missing)
	case "get-videos":
		return GetVideos(ids, slug, missing)
	case "info":
		allText := req.Flag("all-text")
		images := req.Flag("images")
		videoId := req.Flag("video-id")
		return Info(slug, ids, allText, images, videoId)
	case "list":
		mha, err := hoursAtoi(req.ArgVal("modified-hours-ago"))
		if err != nil {
			return err
		}
		modifiedSince := time.Now().Add(-time.Hour * time.Duration(mha)).Unix()
		properties := req.ArgValues("property")
		return List(ids, modifiedSince, pt, mt, properties)
	case "owned":
		return Owned(ids, slug)
	case "search":
		query := make(map[string][]string)
		for _, prop := range vangogh_properties.Searchable() {
			if values, ok := req.Arguments[prop]; ok && len(values) > 0 {
				query[prop] = values
			}
		}
		return Search(query)
	case "size":
		osStrings := req.ArgValues("operating-system")
		langCodes := req.ArgValues("language-code")
		dtStrings := req.ArgValues("download-type")
		return Size(ids, slug, mt, osStrings, langCodes, dtStrings, missing)
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
		if req.Flag("all") {
			data, images, screenshots, videos = true, true, true, true
		}
		data = data && !req.Flag("no-data")
		images = images && !req.Flag("no-images")
		screenshots = screenshots && !req.Flag("no-screenshots")
		videos = videos && req.Flag("no-videos")
		sha, err := hoursAtoi(req.ArgVal("since-hours-ago"))
		if err != nil {
			return err
		}
		return Sync(mt, sha, data, images, screenshots, videos, verbose)
	case "tag":
		operation := req.ArgVal("operation")
		tagName := req.ArgVal("tag-name")
		id := req.ArgVal("id")
		return Tag(operation, tagName, id)
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
