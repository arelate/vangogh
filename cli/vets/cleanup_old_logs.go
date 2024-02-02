package vets

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/hogo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pasu"
)

func CleanupOldLogs(fix bool) error {
	cla := nod.NewProgress("cleaning up old logs...")
	defer cla.End()

	absLogsDir, err := pasu.GetAbsDir(vangogh_local_data.Logs)
	if err != nil {
		return err
	}

	if err := hogo.Cleanup(absLogsDir, fix, cla); err != nil {
		return cla.EndWithError(err)
	}

	cla.EndWithResult("done")

	return nil
}
