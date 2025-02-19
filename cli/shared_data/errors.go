package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/redux"
	"time"
)

func WriteTypeErrors(itemErrors map[string]error, rdx redux.Writeable) error {

	if len(itemErrors) == 0 {
		return nil
	}

	typeErrors := make(PropertyIdValues)
	for _, property := range vangogh_integration.TypeErrorProperties() {
		typeErrors[property] = make(map[string][]string)
	}

	for id, typeErr := range itemErrors {
		var value string
		for _, property := range vangogh_integration.TypeErrorProperties() {
			switch property {
			case vangogh_integration.TypeErrorMessageProperty:
				value = typeErr.Error()
			case vangogh_integration.TypeErrorDateProperty:
				value = time.Now().UTC().Format(time.RFC3339)
			}
			typeErrors[property][id] = []string{value}
		}
	}

	var err error

	for property, idValues := range typeErrors {
		if err = rdx.BatchReplaceValues(property, idValues); err != nil {
			return err
		}
	}

	return nil
}
