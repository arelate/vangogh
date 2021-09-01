package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/output"
	"github.com/boggydigital/vangogh/cmd/url_helpers"
	"net/url"
)

func DigestHandler(u *url.URL) error {
	return Digest(url_helpers.Value(u, "property"))
}

func Digest(property string) error {

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

	var sorted []string

	_, sorted = gost.NewIntSortedStrSetWith(distValues, output.DefaultDesc)

	for _, key := range sorted {
		fmt.Printf("%s: %d items\n", key, distValues[key])
	}

	return nil
}
