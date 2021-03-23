package cmd

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
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

	pt := vangogh_products.Parse(productType)
	mt := gog_media.Parse(media)

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
		return Info(ids, mt, images)
	case "list":
		properties := req.ArgValues("property")
		return List(ids, pt, mt, properties...)
	case "search":
		supportedProperties := []string{
			vangogh_properties.AllTextProperties,
			vangogh_properties.AllImageIdProperties}
		supportedProperties = append(supportedProperties,
			vangogh_properties.AllText()...)
		query := make(map[string]string)
		for _, prop := range supportedProperties {
			if values, ok := req.Arguments[prop]; ok && len(values) > 0 {
				query[prop] = values[0]
			}
		}
		return Search(query)
	case "get-images":
		imageType := req.ArgVal("image-type")
		it := vangogh_images.Parse(imageType)
		all := req.Flag("all")
		return GetImages(ids, it, all)
	case "scrub-data":
		fix := req.Flag("fix")
		return ScrubData(mt, fix)
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
