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

	distinctValues := make([]string, 0)

	for _, id := range exl.All(property) {
		values, ok := exl.GetAll(property, id)
		if !ok || len(values) == 0 {
			continue
		}

		for _, val := range values {
			if val == "" {
				continue
			}
			if !stringsContain(distinctValues, val) {
				distinctValues = append(distinctValues, val)
			}
		}
	}

	sort.Strings(distinctValues)

	for _, distinctValue := range distinctValues {
		fmt.Println(distinctValue)
	}

	return nil
}
