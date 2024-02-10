package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/konpo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pasu"
	"net/url"
)

func BackupHandler(_ *url.URL) error {
	return Backup()
}

func Backup() error {

	ba := nod.NewProgress("backing up local data...")
	defer ba.End()

	abp, err := pasu.GetAbsDir(vangogh_local_data.Backups)
	if err != nil {
		return ba.EndWithError(err)
	}

	amp, err := pasu.GetAbsDir(vangogh_local_data.Metadata)
	if err != nil {
		return ba.EndWithError(err)
	}

	if err := konpo.Compress(amp, abp); err != nil {
		return ba.EndWithError(err)
	}

	ba.EndWithResult("done")

	ca := nod.NewProgress("cleaning up old backups...")
	defer ca.End()

	if err := konpo.Cleanup(abp, true, ca); err != nil {
		return ca.EndWithError(err)
	}

	ca.EndWithResult("done")

	return nil
}
