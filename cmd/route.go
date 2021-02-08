package cmd

import (
	"github.com/boggydigital/clo"
)

func Route(req *clo.Request, defs *clo.Definitions) error {
	if req == nil {
		return clo.Route(nil, defs)
	}
	switch req.Command {
	case "auth":
		username := req.ArgVal("username")
		password := req.ArgVal("password")
		return Authenticate(username, password)
	case "fetch":
		ids := req.ArgValues("id")
		productType := req.ArgVal("type")
		media := req.ArgVal("media")
		missing := req.Flag("missing")
		return Fetch(ids, productType, media, missing)
	case "list":
		productType := req.ArgVal("type")
		media := req.ArgVal("media")
		return List(productType, media)
	case "test":
		return Test()
	default:
		return clo.Route(req, defs)
	}
}
