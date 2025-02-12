package vets

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/backups"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
)

func CleanupOldLogs(fix bool) error {
	cla := nod.NewProgress("cleaning up old logs...")
	defer cla.EndWithResult("done")

	absLogsDir, err := pathways.GetAbsDir(vangogh_integration.Logs)
	if err != nil {
		return err
	}

	if err := backups.Cleanup(absLogsDir, fix, cla); err != nil {
		return err
	}

	return nil
}
