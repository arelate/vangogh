package cli

import (
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/nod"
	"net/url"
	"runtime/debug"
)

func VersionHandler(_ *url.URL) error {
	va := nod.Begin("checking vangogh version...")
	defer va.Done()

	if reqs.GitTag == "" {
		summary := make(map[string][]string)
		if bi, ok := debug.ReadBuildInfo(); ok {
			values := []string{bi.Main.Version, bi.Main.Path, bi.GoVersion}
			for _, value := range values {
				if value != "" {
					summary["version info:"] = append(summary["version info:"], value)
				}
			}
			va.EndWithSummary("", summary)
		} else {
			va.EndWithResult("unknown version")
		}
	} else {
		va.EndWithResult(reqs.GitTag)
	}
	return nil
}
