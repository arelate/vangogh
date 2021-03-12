package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_values"
	"strings"
)

func Search(mt gog_types.Media, query map[string][]string, properties []string) error {
	ps := make([]string, 0, len(query))
	for p, values := range query {
		ps = append(ps, p)
		for i, req := range values {
			query[p][i] = strings.ToLower(req)
		}
	}

	propStashes, err := vangogh_properties.PropStashes(ps)
	if err != nil {
		return err
	}

	for _, pt := range vangogh_types.AllLocalProductTypes() {

		matchIds := make([]string, 0)

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		for _, id := range vr.All() {
			matchesAll := true

			for _, pp := range ps {

				stash := propStashes[pp]

				val, ok := stash.Get(id)
				if !ok {
					matchesAll = false
				}

				matchesValue := false
				for _, req := range query[pp] {
					if strings.Contains(strings.ToLower(val), req) {
						matchesValue = true
					}
				}

				matchesAll = matchesAll && matchesValue

				if !matchesAll {
					break
				}

				//fmt.Println(id, pp, val)
			}

			if matchesAll {
				matchIds = append(matchIds, id)
			}
		}

		if len(matchIds) > 0 {
			fmt.Printf("%s (%s):\n", pt, mt)
			if err := List(matchIds, pt, mt, properties...); err != nil {
				return err
			}
		}
	}

	return nil
}
