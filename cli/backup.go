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

	abp, err := vangogh_local_data.GetAbsDir(vangogh_local_data.Backups)
	if err != nil {
		return err
	}

	if _, err := os.Stat(abp); os.IsNotExist(err) {
		if err := os.MkdirAll(abp, 0755); err != nil {
			return ba.EndWithError(err)
		}
	}

	return Export(abp)
}
