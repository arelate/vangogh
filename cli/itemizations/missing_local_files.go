package itemizations

import (
	"iter"
)

func missingLocalFiles(
	all iter.Seq[string],
	localSet map[string]bool,
	getById func(id string) ([]string, bool),
	exclude func(id string) bool) (map[string]bool, error) {

	idSet := make(map[string]bool)
	var err error

	for id := range all {
		items, ok := getById(id)
		if !ok || len(items) == 0 {
			continue
		}

		for _, item := range items {
			if exclude != nil && exclude(item) {
				continue
			}
			if !localSet[item] {
				idSet[id] = true
				break
			}
		}
	}

	return idSet, err
}
