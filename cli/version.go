package cli

import (
	"fmt"
	"github.com/arelate/vangogh/cli/reqs"
	"net/url"
	"runtime/debug"
)

func VersionHandler(_ *url.URL) error {
	if reqs.GitTag == "" {
		if bi, ok := debug.ReadBuildInfo(); ok {
			fmt.Println(bi)
		} else {
			fmt.Println("unknown version")
		}
	} else {
		fmt.Println(reqs.GitTag)
	}
	return nil
}
