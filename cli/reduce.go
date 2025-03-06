package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reductions"
	"github.com/boggydigital/nod"
	"net/url"
)

func ReduceHandler(u *url.URL) error {
	var since int64
	if vangogh_integration.FlagFromUrl(u, "since-hours-ago") {
		var err error
		since, err = vangogh_integration.SinceFromUrl(u)
		if err != nil {
			return err
		}
	}
	return Reduce(
		since,
		vangogh_integration.PropertiesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "properties-only"))
}

func Reduce(since int64, properties []string, propertiesOnly bool) error {

	ra := nod.Begin("reducing properties...")
	defer ra.Done()

	if !propertiesOnly {

		if err := reductions.Owned(); err != nil {
			return err
		}
	}

	return reductions.Cascade()
}
