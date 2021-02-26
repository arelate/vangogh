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
	ids := req.ArgValues("id")

	pt := vangogh_types.ParseProductType(productType)
	mt := gog_types.ParseMedia(media)

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
		return List(ids, title, productType, media)
	case "download":
		downloadType := req.ArgVal("download-type")
		all := req.Flag("all")
		return Download(ids, productType, media, downloadType, all)
	case "sync":
		return Sync(mt)
	case "summarize":
		return Summarize(productType, media)
	case "test":
		return Test()
	default:
		return clo.Route(req, defs)
	}
}
