package cmd

import (
	"github.com/boggydigital/clo"
)

func Route(req *clo.Request, defs *clo.Definitions) error {
	if req == nil {
		return clo.Route(nil, defs)
	}

	ids := req.ArgValues("id")
	media := req.ArgVal("media")
	productType := req.ArgVal("product-type")

	switch req.Command {
	case "auth":
		username := req.ArgVal("username")
		password := req.ArgVal("password")
		return Authenticate(username, password)
	case "fetch":
		missing := req.Flag("missing")
		return Fetch(ids, productType, media, missing)
	case "list":
		title := req.ArgVal("title")
		return List(ids, title, productType, media)
	case "download":
		downloadType := req.ArgVal("download-type")
		all := req.Flag("all")
		return Download(ids, productType, media, downloadType, all)
	case "sync":
		return Sync(media)
	case "test":
		return Test()
	default:
		return clo.Route(req, defs)
	}
}
