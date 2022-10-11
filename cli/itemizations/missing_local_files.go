package itemizations

import (
	"github.com/boggydigital/nod"
)

func missingLocalFiles(
	all []string,
	localSet map[string]bool,
	getById func(id string) ([]string, bool),
	exclude func(id string) bool,
	tpw nod.TotalProgressWriter) (map[string]bool, error) {

	idSet := make(map[string]bool)
	var err error

	if tpw != nil {
		tpw.TotalInt(len(all))
	}

	for _, id := range all {
		items, ok := getById(id)
		if !ok || len(items) == 0 {
			if tpw != nil {
				tpw.Increment()
			}
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

		if tpw != nil {
			tpw.Increment()
		}
	}

	return idSet, err
}
