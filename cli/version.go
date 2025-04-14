package cli

import (
	"fmt"
	"github.com/arelate/vangogh/cli/reqs"
	"net/url"
)

func VersionHandler(_ *url.URL) error {
	if reqs.GitTag == "" {
		fmt.Println("development version")
	} else {
		fmt.Println(reqs.GitTag)
	}
	return nil
}
