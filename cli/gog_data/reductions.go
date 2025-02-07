package gog_data

import (
	"github.com/boggydigital/redux"
)

func initReductions(properties ...string) propertyIdValues {
	piv := make(propertyIdValues)
	for _, property := range properties {
		piv[property] = make(map[string][]string)
	}
	return piv
}

func writeReductions(rdx redux.Writeable, piv propertyIdValues) error {
	for property, keyValues := range piv {
		if err := rdx.BatchReplaceValues(property, keyValues); err != nil {
			return err
		}
	}
	return nil
}
