package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"sort"
)

func Digest(property string) error {

	exl, err := vangogh_extracts.NewList(property)
	if err != nil {
		return err
	}

	distValues := make(map[string]bool, 0)

	for _, id := range exl.All(property) {
		values, ok := exl.GetAll(property, id)
		if !ok || len(values) == 0 {
			continue
		}

		for _, val := range values {
			if val == "" {
				continue
			}
			distValues[val] = true
		}
	}

	keys := make([]string, 0, len(distValues))
	for key, _ := range distValues {
		keys = append(keys, key)
	}

	sort.Strings(keys)

	for _, key := range keys {
		fmt.Println(key)
	}

	return nil
}
