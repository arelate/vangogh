package vets

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"os"
	"path/filepath"
)

func FilesInRecycleBin(fix bool) error {

	srba := nod.Begin("checking files in recycle bin...")
	defer srba.End()

	recycleBinFiles, err := vangogh_local_data.RecycleBinFiles()
	if err != nil {
		return srba.EndWithError(err)
	}
	recycleBinDirs, err := vangogh_local_data.RecycleBinDirs()
	if err != nil {
		return srba.EndWithError(err)
	}

	if len(recycleBinFiles) == 0 && len(recycleBinDirs) == 0 {
		srba.EndWithResult("none found")
	} else {

		srba.EndWithResult("%d file(s) found", len(recycleBinFiles))

		if fix {
			rfa := nod.NewProgress(" emptying recycle bin...")
			rfa.TotalInt(len(recycleBinFiles))
			for file := range recycleBinFiles {
				if err := os.Remove(filepath.Join(vangogh_local_data.AbsRecycleBinDir(), file)); err != nil {
					return rfa.EndWithError(err)
				}
				rfa.Increment()
			}
			rfa.EndWithResult("done")

			//remove empty directories after fixing files
			dirLens := make(map[string]int)

			for dir := range recycleBinDirs {
				dirLens[dir] = len(dir)
			}

			sortedDirs := vangogh_local_data.SortStrIntMap(dirLens, true)

			rda := nod.NewProgress(" removing leftover directories...")
			rda.TotalInt(len(sortedDirs))

			for _, dir := range sortedDirs {
				if err := os.Remove(dir); err != nil {
					return rda.EndWithError(err)
				}
				rda.Increment()
			}
			rda.EndWithResult("done")
		}
	}

	return nil
}
