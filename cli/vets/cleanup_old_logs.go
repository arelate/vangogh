package vets

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/backups"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
)

func CleanupOldLogs(fix bool) error {
	cla := nod.NewProgress("cleaning up old logs...")
	defer cla.End()

	absLogsDir, err := pathways.GetAbsDir(vangogh_local_data.Logs)
	if err != nil {
		return err
	}

	if err := backups.Cleanup(absLogsDir, fix, cla); err != nil {
		return cla.EndWithError(err)
	}

	cla.EndWithResult("done")

	return nil
}
