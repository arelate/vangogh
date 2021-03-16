package cmd

import (
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/boggydigital/clo"
	"github.com/boggydigital/vangogh/internal"
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
		return GetData(ids, denyIds, pt, mt, missing, verbose)
	case "list":
		properties := req.ArgValues("property")
		return List(ids, pt, mt, properties...)
	case "get-images":
		imageType := req.ArgVal("image-type")
		it := vangogh_types.ParseImageType(imageType)
		all := req.Flag("all")
		return GetImages(ids, it, all)
	case "sync":
		return Sync(mt, verbose)
	case "extract":
		properties := req.ArgValues("properties")
		return Extract(mt, properties)
	case "search":
		text := req.ArgVal("text")
		imageId := req.ArgVal("image-id")
		return Search(mt, text, imageId)
	case "info":
		images := req.Flag("images")
		return Info(ids, mt, images)
	default:
		return clo.Route(req, defs)
	}
}
