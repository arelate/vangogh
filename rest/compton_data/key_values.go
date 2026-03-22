package compton_data

import (
	"errors"
	"io"

	"github.com/boggydigital/kevlar"
)

func GetKeyValuesBytes(id, property string, keyValues map[string]kevlar.KeyValues) ([]byte, error) {

	var kv kevlar.KeyValues
	var ok bool

	if kv, ok = keyValues[property]; !ok {
		return nil, errors.New("keyValues not initialized for " + property)
	}

	rc, err := kv.Get(id)
	if err != nil {
		return nil, err
	}
	defer rc.Close()

	return io.ReadAll(rc)
}

func HasKeyValuesBytes(id, property string, keyValues map[string]kevlar.KeyValues) (bool, error) {
	var kv kevlar.KeyValues
	var ok bool

	if kv, ok = keyValues[property]; !ok {
		return false, errors.New("keyValues not initialized for " + property)
	}

	return kv.Has(id), nil
}
