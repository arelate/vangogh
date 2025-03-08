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
		fkvs := filterEmptyValues(keyValues)
		if err := rdx.BatchReplaceValues(property, fkvs); err != nil {
			return err
		}
	}
	return nil
}

func filterEmptyValues(kv map[string][]string) map[string][]string {
	fkv := make(map[string][]string)
	for key, values := range kv {
		fvs := make([]string, 0, len(values))
		for _, val := range values {
			if val != "" {
				fvs = append(fvs, val)
			}
		}
		if len(fvs) > 0 {
			fkv[key] = fvs
		}
	}
	return fkv
}
