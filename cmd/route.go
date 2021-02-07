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
		return Fetch(ids, productType, media)
	default:
		return clo.Route(req, defs)
	}
}
