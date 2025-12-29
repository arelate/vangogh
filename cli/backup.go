package cli

import (
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/backups"
	"github.com/boggydigital/nod"
)

func BackupHandler(_ *url.URL) error {
	return Backup()
}

func Backup() error {

	ba := nod.NewProgress("backing up local data...")
	defer ba.Done()

	abp := vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Backups)
	amp := vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Metadata)

	if err := backups.Compress(amp, abp); err != nil {
		return err
	}

	ca := nod.NewProgress("cleaning up old backups...")
	defer ca.Done()

	if err := backups.Cleanup(abp, true, ca); err != nil {
		return err
	}

	return nil
}
