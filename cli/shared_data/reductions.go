package shared_data

import (
	"path/filepath"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
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

func InitKeyValues(keyValues ...string) (map[string]kevlar.KeyValues, error) {
	kvs := make(map[string]kevlar.KeyValues)
	var err error
	for _, kv := range keyValues {
		kvDir := filepath.Join(vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Metadata), kv)
		if kvs[kv], err = kevlar.New(kvDir, kevlar.TxtExt); err != nil {
			return nil, err
		}
	}
	return kvs, nil
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

func IsNotEmpty(strs ...string) bool {
	if len(strs) == 0 {
		return false
	}
	for _, s := range strs {
		if s != "" {
			return true
		}
	}
	return false
}
