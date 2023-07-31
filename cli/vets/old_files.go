package vets

import (
	"github.com/boggydigital/nod"
	"os"
	"path/filepath"
	"strings"
	"time"
)

const daysToPreserveFiles = 30

func OldFiles(dir string, entities string, fix bool) error {

	ofa := nod.Begin("looking for old %s...", entities)
	defer ofa.End()

	d, err := os.Open(dir)
	if err != nil {
		return ofa.EndWithError(err)
	}
	defer d.Close()

	filenames, err := d.Readdirnames(-1)
	if err != nil {
		return ofa.EndWithError(err)
	}

	earliest := time.Now().Add(-daysToPreserveFiles * 24 * time.Hour)
	oldFiles := make([]string, 0)

	for _, fn := range filenames {

		fnse := fn
		for filepath.Ext(fnse) != "" {
			fnse = strings.TrimSuffix(fnse, filepath.Ext(fnse))
		}
		ft, err := time.Parse(nod.TimeFormat, fnse)
		if err != nil {
			return ofa.EndWithError(err)
		}

		if ft.After(earliest) {
			continue
		}

		oldFiles = append(oldFiles, fn)
	}

	if len(oldFiles) == 0 {
		ofa.EndWithResult("none found")
	} else {
		ofa.EndWithResult("found %d old %s", len(oldFiles), entities)

		if fix {
			rofa := nod.NewProgress("removing old %s...", entities)
			rofa.TotalInt(len(oldFiles))
			for _, fn := range oldFiles {
				filename := filepath.Join(dir, fn)
				if err := os.Remove(filename); err != nil {
					return rofa.EndWithError(err)
				}
				rofa.Increment()
			}
			rofa.EndWithResult("done")
		}
	}

	return nil
}
