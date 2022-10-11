package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/url"
	"os"
)

func BackupHandler(u *url.URL) error {
	return Backup()
}

func Backup() error {

	ba := nod.Begin("backing up local data...")
	defer ba.End()

	if _, err := os.Stat(vangogh_local_data.AbsBackupDir()); os.IsNotExist(err) {
		if err := os.MkdirAll(vangogh_local_data.AbsBackupDir(), 0755); err != nil {
			return ba.EndWithError(err)
		}
	}

	return Export(
		vangogh_local_data.AbsReduxDir(),
		vangogh_local_data.AbsBackupDir())
}
