package cmd

import "github.com/boggydigital/gost"

func idsMissingLocalFiles(
	all []string,
	localSet gost.StrSet,
	getById func(id string) ([]string, bool),
	exclude func(id string) bool) (gost.StrSet, error) {

	idSet := gost.NewStrSet()
	var err error

	for _, id := range all {
		items, ok := getById(id)
		if !ok || len(items) == 0 {
			continue
		}

		for _, item := range items {
			if exclude != nil && exclude(item) {
				continue
			}
			if !localSet.Has(item) {
				idSet.Add(id)
				break
			}
		}
	}

	return idSet, err
}
