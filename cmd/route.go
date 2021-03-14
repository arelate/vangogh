package cmd

import (
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
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
		downloadType := req.ArgVal("download-type")
		dt := vangogh_types.ParseDownloadType(downloadType)
		all := req.Flag("all")
		return GetImages(ids, mt, dt, all)
	case "sync":
		return Sync(mt, verbose)
	case "extract":
		return Extract()
	case "stash":
		properties := req.ArgValues("property")
		return Stash(pt, mt, properties)
	case "distill":
		properties := req.ArgValues("property")
		return Distill(pt, mt, properties)
	case "search":
		properties := req.ArgValues("property")
		// TODO: filter argValue to AllProperties
		query := make(map[string][]string, 0)
		for _, prop := range vangogh_properties.AllStashedProperties() {
			if values, ok := req.Arguments[prop]; ok {
				query[prop] = values
			}
		}
		productTypes := req.ArgValues("product-type")
		pts := make([]vangogh_types.ProductType, 0, len(productTypes))
		for _, productType := range productTypes {
			ppt := vangogh_types.ParseProductType(productType)
			if ppt == vangogh_types.UnknownProductType {
				continue
			}
			pts = append(pts, ppt)
		}
		return Search(pts, mt, query, properties)
	default:
		return clo.Route(req, defs)
	}
}
