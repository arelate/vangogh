package cli

import (
	"fmt"
	"net/url"
)

var (
	GitTag string
)

func VersionHandler(u *url.URL) error {
	if GitTag == "" {
		fmt.Println("unknown version")
	} else {
		fmt.Println(GitTag)
	}
	return nil
}
