package cli_api

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/output"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
)

func DigestHandler(u *url.URL) error {
	return Digest(url_helpers.Value(u, "property"))
}

func Digest(property string) error {

	da := nod.Begin("digesting...")
	defer da.End()

	exl, err := vangogh_extracts.NewList(property)
	if err != nil {
		return err
	}

	distValues := make(map[string]int, 0)

	for _, id := range exl.All(property) {
		values, ok := exl.GetAll(property, id)
		if !ok || len(values) == 0 {
			continue
		}

		for _, val := range values {
			if val == "" {
				continue
			}
			distValues[val] = distValues[val] + 1
		}
	}

	keys := make([]string, 0, len(distValues))
	for key, _ := range distValues {
		keys = append(keys, key)
	}

	_, sorted := gost.NewIntSortedStrSetWith(distValues, output.DefaultDesc)

	summary := make(map[string][]string)
	summary[""] = make([]string, 0, len(sorted))
	for _, key := range sorted {
		summary[""] = append(summary[""], fmt.Sprintf("%s: %d items", key, distValues[key]))
	}

	da.EndWithSummary(summary)

	return nil
}
