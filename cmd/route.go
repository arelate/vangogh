package cmd

import (
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/vangogh/internal"
	"time"
)

func Route(req *clo.Request, defs *clo.Definitions) error {
	if req == nil {
		return clo.Route(nil, defs)
	}

	verbose := req.Flag("verbose")

	productType := req.ArgVal("product-type")
	media := req.ArgVal("media")

	pt := vangogh_types.ParseProductType(productType)
	mt := gog_types.ParseMedia(media)

	ids := req.ArgValues("id")

	switch req.Command {
	case "auth":
		username := req.ArgVal("username")
		password := req.ArgVal("password")
		return Authenticate(username, password)
	case "get-data":
		missing := req.Flag("missing")
		denyIdsFile := req.ArgVal("deny-ids-file")
		denyIds := internal.ReadLines(denyIdsFile)
		return GetData(ids, denyIds, pt, mt, time.Now().Unix(), missing, verbose)
	case "info":
		images := req.Flag("images")
		return Info(ids, images)
	case "list":
		properties := req.ArgValues("property")
		return List(ids, pt, mt, properties...)
	case "search":
		text := req.ArgVal("text")
		title := req.ArgVal("title")
		developer := req.ArgVal("developer")
		publisher := req.ArgVal("publisher")
		imageId := req.ArgVal("image-id")
		return Search(text, title, developer, publisher, imageId)
	case "get-images":
		imageType := req.ArgVal("image-type")
		it := vangogh_types.ParseImageType(imageType)
		all := req.Flag("all")
		return GetImages(ids, it, all)
	case "sync":
		images := req.Flag("images")
		screenshots := req.Flag("screenshots")
		noData := req.Flag("no-data")
		all := req.Flag("all")
		if all {
			images = true
			screenshots = true
		}
		return Sync(mt, noData, images, screenshots, verbose)
	case "extract":
		properties := req.ArgValues("properties")
		return Extract(mt, properties)
	default:
		return clo.Route(req, defs)
	}
}
