package compton_data

import (
	"io"
	"path/filepath"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
)

var (
	keyValues         = make(map[string]kevlar.KeyValues)
	keyValuesModTimes = make(map[string]int64)
)

func refreshKeyValues(id, property string) (kevlar.KeyValues, error) {

	var refresh bool

	if kv, ok := keyValues[property]; !ok {
		refresh = true
	} else {
		if mt, sure := keyValuesModTimes[property]; !sure || mt < kv.LogModTime(id) {
			refresh = true
		}
	}

	var err error

	if refresh {
		kvDir := filepath.Join(vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Metadata), property)
		keyValues[property], err = kevlar.New(kvDir, kevlar.TxtExt)
		keyValuesModTimes[property] = time.Now().UTC().Unix()
		if err != nil {
			return nil, err
		}
	}

	return keyValues[property], nil
}

func GetKeyValuesBytes(id, property string) ([]byte, error) {

	kv, err := refreshKeyValues(id, property)
	if err != nil {
		return nil, err
	}

	if !kv.Has(id) {
		return nil, nil
	}

	rc, err := kv.Get(id)
	if err != nil {
		return nil, err
	}
	defer rc.Close()

	return io.ReadAll(rc)
}

func HasKeyValuesBytes(id, property string) (bool, error) {

	kv, err := refreshKeyValues(id, property)
	if err != nil {
		return false, err
	}

	return kv.Has(id), nil
}
