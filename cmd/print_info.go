package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"strings"
)

func printInfo(
	id string,
	isNew bool,
	propertyFilter map[string]string,
	properties []string,
	exl *vangogh_extracts.ExtractsList) {

	title, ok := exl.Get(vangogh_properties.TitleProperty, id)
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
		values, ok := exl.GetAll(prop, id)
		if !ok || len(values) == 0 {
			continue
		}
		filter := propertyFilter[prop]

		if len(values) > 1 && vangogh_properties.JoinPreferred(prop) {
			joinedValue := strings.Join(values, ",")
			if filter != "" && !strings.Contains(strings.ToLower(joinedValue), filter) {
				continue
			}
			fmt.Printf(" %s:%s\n", prop, joinedValue)
			continue
		}

		for _, val := range values {
			if filter != "" && !strings.Contains(strings.ToLower(val), filter) {
				continue
			}
			fmt.Printf(" %s:%s\n", prop, val)
		}
	}
}
