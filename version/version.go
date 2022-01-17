package version

import (
	"github.com/boggydigital/nod"
	"net/url"
)

var (
	Version   string
	Commit    string
	BuildDate string
)

func VersionHander(u *url.URL) error {
	va := nod.Begin("")

	va.EndWithSummary("app information:", map[string][]string{
		"version":    {Version},
		"commit":     {Commit},
		"build date": {BuildDate},
	})

	return nil
}
