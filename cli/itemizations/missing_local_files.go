package itemizations

import (
	"iter"
)

func missingLocalFiles(
	all iter.Seq[string],
	localSet map[string]any,
	getById func(id string) ([]string, bool),
	exclude func(id string) bool) (map[string]any, error) {

	idSet := make(map[string]any)
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
			if _, present := localSet[item]; !present {
				idSet[id] = nil
				break
			}
		}
	}

	return idSet, err
}
