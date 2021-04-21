package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/froth"
	"sort"
)

func Digest(property string) error {

	extracts, err := froth.NewStash(vangogh_urls.ExtractsDir(), property)
	if err != nil {
		return err
	}

	distinctValues := make([]string, 0)

	for _, id := range extracts.All() {
		values, ok := extracts.GetAll(id)
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
