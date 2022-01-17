package version

import (
	"github.com/boggydigital/nod"
	"net/url"
)

var (
	//TODO: Explore replacing with runtime/debug.ReadBuildInfo when 1.18 releases
	//and simplify make and docker build scripts
	Version   string = "undefined"
	Commit    string = "undefined"
	BuildDate string = "undefined"
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
