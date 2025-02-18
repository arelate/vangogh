package shared_data

import (
	"github.com/boggydigital/redux"
)

type PropertyIdValues map[string]map[string][]string

func InitReductions(properties ...string) PropertyIdValues {
	piv := make(PropertyIdValues)
	for _, property := range properties {
		piv[property] = make(map[string][]string)
	}
	return piv
}

func WriteReductions(rdx redux.Writeable, piv PropertyIdValues) error {
	for property, keyValues := range piv {
		if err := rdx.BatchReplaceValues(property, keyValues); err != nil {
			return err
		}
	}
	return nil
}
