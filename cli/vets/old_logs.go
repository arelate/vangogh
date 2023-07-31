package vets

import (
	"github.com/arelate/vangogh/cli/dirs"
	"github.com/boggydigital/nod"
	"os"
	"path/filepath"
	"strings"
	"time"
)

const daysToPreserveLogs = 30

func OldLogs(fix bool) error {

	ola := nod.Begin("looking for oldLogs logs...")
	defer ola.End()

	logsDir, err := os.Open(dirs.LogsDir)
	if err != nil {
		return ola.EndWithError(err)
	}

	logsNames, err := logsDir.Readdirnames(-1)
	if err != nil {
		return ola.EndWithError(err)
	}

	earliest := time.Now().Add(-daysToPreserveLogs * 24 * time.Hour)
	oldLogs := make([]string, 0)

	for _, ln := range logsNames {

		lfn := strings.TrimSuffix(ln, filepath.Ext(ln))
		lt, err := time.Parse(nod.TimeFormat, lfn)
		if err != nil {
			return ola.EndWithError(err)
		}

		if lt.After(earliest) {
			continue
		}

		oldLogs = append(oldLogs, ln)
	}

	if len(oldLogs) == 0 {
		ola.EndWithResult("none found")
	} else {
		ola.EndWithResult("found %d log(s) >%d days old", len(oldLogs), daysToPreserveLogs)

		if fix {
			fola := nod.NewProgress("removing old logs...")
			fola.TotalInt(len(oldLogs))
			for _, ln := range oldLogs {
				logFilename := filepath.Join(dirs.LogsDir, ln)
				if err := os.Remove(logFilename); err != nil {
					return fola.EndWithError(err)
				}
				fola.Increment()
			}
			fola.EndWithResult("done")
		}
	}

	return nil
}
