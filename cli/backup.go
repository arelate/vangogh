package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/url"
	"os"
)

func BackupHandler(_ *url.URL) error {
	return Backup()
}

func Backup() error {

	ba := nod.Begin("backing up local data...")
	defer ba.End()

	abd := vangogh_local_data.AbsBackupsDir()

	if _, err := os.Stat(abd); os.IsNotExist(err) {
		if err := os.MkdirAll(abd, 0755); err != nil {
			return ba.EndWithError(err)
		}
	}

	return Export(abd)
}
