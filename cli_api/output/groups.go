package output

import (
	"fmt"
	"github.com/arelate/vangogh_properties"
	"sort"
)

const (
	DefaultSort = vangogh_properties.TitleProperty
	DefaultDesc = false
)

func Groups(
	groupIds map[string][]string) error {

	//propSet := gost.NewStrSetWith(vangogh_properties.TitleProperty)

	groups := make([]string, 0, len(groupIds))
	for grp, _ := range groupIds {
		groups = append(groups, grp)
	}

	sort.Strings(groups)

	//exl, err := vangogh_extracts.NewList(propSet.All()...)
	//if err != nil {
	//	return err
	//}

	for _, grp := range groups {
		if len(groupIds[grp]) == 0 {
			continue
		}

		fmt.Printf(" %s:\n", grp)

		//if err := Items(groupIds[grp], nil, propSet.All(), exl); err != nil {
		//	return err
		//}
	}

	return nil
}
