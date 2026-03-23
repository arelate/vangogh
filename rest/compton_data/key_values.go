package compton_data

import (
	"io"
	"path/filepath"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
)

func GetKeyValuesBytes(id, property string) ([]byte, error) {

	kvDir := filepath.Join(vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Metadata), property)

	kv, err := kevlar.New(kvDir, kevlar.TxtExt)
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

	kvDir := filepath.Join(vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Metadata), property)

	kv, err := kevlar.New(kvDir, kevlar.TxtExt)
	if err != nil {
		return false, err
	}
	return kv.Has(id), nil
}
