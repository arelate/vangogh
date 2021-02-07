package cmd

import (
	"github.com/boggydigital/clo"
)

func Route(req *clo.Request, defs *clo.Definitions) error {
	if req == nil {
		return clo.Route(nil, defs)
	}
	switch req.Command {
	case "fetch":
		ftype, media := req.ArgVal("type"), req.ArgVal("media")
		return Fetch(ftype, media)
	default:
		return clo.Route(req, defs)
	}
}
