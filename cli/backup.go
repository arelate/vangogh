package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/packer"
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

	if err := packer.Pack(amp, abp, ba); err != nil {
		return ba.EndWithError(err)
	}

	ba.EndWithResult("done")

	return nil
}
