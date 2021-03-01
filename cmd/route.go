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
		title := req.ArgVal("title")
		developer := req.ArgVal("developer")
		publisher := req.ArgVal("publisher")
		return List(ids, pt, mt, title, developer, publisher)
	case "download":
		downloadType := req.ArgVal("download-type")
		dt := vangogh_types.ParseDownloadType(downloadType)
		all := req.Flag("all")
		return Download(ids, pt, mt, dt, all)
	case "sync":
		return Sync(mt)
	case "memorize":
		properties := req.ArgValues("property")
		return Memorize(pt, mt, properties)
	default:
		return clo.Route(req, defs)
	}
}
