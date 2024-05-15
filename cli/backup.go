package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/backups"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/url"
)

func BackupHandler(_ *url.URL) error {
	return Backup()
}

func Backup() error {

	ba := nod.NewProgress("backing up local data...")
	defer ba.End()

	abp, err := pathways.GetAbsDir(vangogh_local_data.Backups)
	if err != nil {
		return ba.EndWithError(err)
	}

	amp, err := pathways.GetAbsDir(vangogh_local_data.Metadata)
	if err != nil {
		return ba.EndWithError(err)
	}

	if err := backups.Compress(amp, abp); err != nil {
		return ba.EndWithError(err)
	}

	ba.EndWithResult("done")

	ca := nod.NewProgress("cleaning up old backups...")
	defer ca.End()

	if err := backups.Cleanup(abp, true, ca); err != nil {
		return ca.EndWithError(err)
	}

	ca.EndWithResult("done")

	return nil
}
