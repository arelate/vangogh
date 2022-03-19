package app

import (
	"fmt"
	"github.com/boggydigital/nod"
	"net/url"
	"runtime/debug"
)

func VersionHandler(u *url.URL) error {
	va := nod.Begin("")

	if bi, ok := debug.ReadBuildInfo(); ok {
		va.EndWithResult("%v", bi)
	} else {
		va.EndWithError(fmt.Errorf("unable to read build info"))
	}

	return nil
}
