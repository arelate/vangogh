package cli

import (
	"fmt"
	"net/url"
)

var (
	GitTag string
)

func VersionHandler(_ *url.URL) error {
	if GitTag == "" {
		fmt.Println("unknown version")
	} else {
		fmt.Println(GitTag)
	}
	return nil
}
