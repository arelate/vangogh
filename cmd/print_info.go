package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/froth"
	"strings"
)

func printInfo(
	id string,
	isNew bool,
	highlightValues map[string]string,
	properties []string,
	propExtracts map[string]*froth.Stash) {

	titleExtracts := propExtracts[vangogh_properties.TitleProperty]
	title, ok := titleExtracts.Get(id)
	if !ok {
		fmt.Printf("product %s not found\n", id)
		return
	}

	if isNew {
		title = fmt.Sprintf("[NEW] %s", title)
	}

	fmt.Println(id, title)

	for _, prop := range properties {
		if prop == vangogh_properties.IdProperty ||
			prop == vangogh_properties.TitleProperty {
			continue
		}
		values, ok := propExtracts[prop].GetAll(id)
		if !ok || len(values) == 0 {
			continue
		}
		if len(values) > 1 && vangogh_properties.JoinPreferred(prop) {
			fmt.Printf(" %s:%s\n", prop, strings.Join(values, ","))
			continue
		}
		for _, val := range values {
			highlight := highlightValues[prop]
			if highlight != "" && !strings.Contains(strings.ToLower(val), highlight) {
				continue
			}
			fmt.Printf(" %s:%s\n", prop, val)
		}
	}
}
