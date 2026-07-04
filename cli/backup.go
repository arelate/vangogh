package cli

import (
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/camino"
	"github.com/boggydigital/nod"
)

func BackupHandler(_ *url.URL) error {
	return Backup()
}

func Backup() error {

	ba := nod.NewProgress("backing up local data...")
	defer ba.Done()

	if err := camino.Compress(vangogh_integration.Metadata, vangogh_integration.Backups); err != nil {
		return err
	}

	ca := nod.NewProgress("cleaning up old backups...")
	defer ca.Done()

	if err := camino.CleanupTimed(vangogh_integration.Backups, true); err != nil {
		return err
	}

	return nil
}
