package cmd

import (
	"github.com/boggydigital/clo"
)

func Route(req *clo.Request) error {
	if req == nil {
		return clo.Route(nil)
	}
	switch req.Command {
	case "fetch":
		productType, media := req.GetValue("type"), req.GetValue("media")
		return Fetch(productType, media)
	default:
		return clo.Route(req)
	}
}
