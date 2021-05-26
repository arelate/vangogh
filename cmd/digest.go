package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/boggydigital/vangogh/internal"
	"sort"
)

func Digest(property string, desc, sortByKey bool) error {

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

	if sortByKey {

		keys := make([]string, 0, len(distValues))
		for key, _ := range distValues {
			keys = append(keys, key)
		}

		sort.Strings(keys)

		for i := 0; i < len(keys); i++ {
			key := keys[i]
			if desc {
				key = keys[len(keys)-1-i]
			}

			fmt.Printf("%s: %d items\n", key, distValues[key])
		}

	} else {

		siList := internal.NewSortList(distValues, desc)
		siList.Sort()

		for _, kv := range siList.KeyValues() {
			fmt.Printf("%s: %d items\n", kv.Key, kv.Val)
		}
	}

	return nil
}
