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
	case "fetch":
		missing := req.Flag("missing")
		denyIdsFile := req.ArgVal("deny-ids-file")
		return Fetch(ids, internal.ReadLines(denyIdsFile), pt, mt, missing)
	case "list":
		properties := req.ArgValues("property")
		return List(ids, pt, mt, properties...)
	case "download":
		downloadType := req.ArgVal("download-type")
		dt := vangogh_types.ParseDownloadType(downloadType)
		all := req.Flag("all")
		return Download(ids, mt, dt, all)
	case "sync":
		return Sync(mt)
	case "stash":
		properties := req.ArgValues("property")
		return Stash(pt, mt, properties)
	case "distill":
		properties := req.ArgValues("property")
		return Distill(pt, mt, properties)
	default:
		return clo.Route(req, defs)
	}
}
