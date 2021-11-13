package checks

import (
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"os"
	"path/filepath"
)

func FilesInRecycleBin(fix bool) error {

	srba := nod.Begin("checking files in recycle bin...")
	defer srba.End()

	recycleBinFiles, err := vangogh_urls.RecycleBinFiles()
	if err != nil {
		return srba.EndWithError(err)
	}
	recycleBinDirs, err := vangogh_urls.RecycleBinDirs()
	if err != nil {
		return srba.EndWithError(err)
	}

	if recycleBinFiles.Len() == 0 && len(recycleBinDirs) == 0 {
		srba.EndWithResult("none found")
	} else {

		srba.EndWithResult("%d file(s) found", recycleBinFiles.Len())

		if fix {
			rfa := nod.NewProgress(" emptying recycle bin...")
			rfa.TotalInt(recycleBinFiles.Len())
			for file := range recycleBinFiles {
				if err := os.Remove(filepath.Join(vangogh_urls.RecycleBinDir(), file)); err != nil {
					return rfa.EndWithError(err)
				}
				rfa.Increment()
			}
			rfa.EndWithResult("done")

			//remove empty directories after fixing files
			sortDirs := gost.NewSortStrSet()
			dirLens := make(map[string]int)

			for _, dir := range recycleBinDirs.All() {
				sortDirs.Add(dir)
				dirLens[dir] = len(dir)
			}

			sortedDirs := sortDirs.SortByIntVal(dirLens, true)

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
