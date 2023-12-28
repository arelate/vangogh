package cli

import (
	"fmt"
	v1 "github.com/arelate/vangogh/rest"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/url"
)

func DigestHandler(u *url.URL) error {
	return Digest(
		vangogh_local_data.PropertyFromUrl(u))
}

func Digest(property string) error {

	da := nod.Begin("digesting...")
	defer da.End()

	rdx, err := vangogh_local_data.NewReduxReader(property)
	if err != nil {
		return err
	}

	distValues := v1.PropertyValuesCounts(rdx, property)

	keys := make([]string, 0, len(distValues))
	for key := range distValues {
		keys = append(keys, key)
	}

	sorted := vangogh_local_data.SortStrIntMap(distValues, vangogh_local_data.DefaultDesc)

	summary := make(map[string][]string)
	summary[""] = make([]string, 0, len(sorted))
	for _, key := range sorted {
		summary[""] = append(summary[""], fmt.Sprintf("%s: %d items", key, distValues[key]))
	}

	da.EndWithSummary("digested properties:", summary)

	return nil
}
