package vets

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"os"
	"path/filepath"
)

func FilesInRecycleBin(fix bool) error {

	srba := nod.Begin("checking files in recycle bin...")
	defer srba.Done()

	recycleBinFiles, err := vangogh_integration.RecycleBinFiles()
	if err != nil {
		return err
	}
	recycleBinDirs, err := vangogh_integration.RecycleBinDirs()
	if err != nil {
		return err
	}

	if len(recycleBinFiles) == 0 && len(recycleBinDirs) == 0 {
		srba.EndWithResult("none found")
	} else {

		srba.EndWithResult("%d file(s) found", len(recycleBinFiles))

		if fix {
			rfa := nod.NewProgress(" emptying recycle bin...")
			rbdp, err := pathways.GetAbsDir(vangogh_integration.RecycleBin)
			if err != nil {
				return err
			}
			rfa.TotalInt(len(recycleBinFiles))
			for file := range recycleBinFiles {
				if err := os.Remove(filepath.Join(rbdp, file)); err != nil {
					return err
				}
				rfa.Increment()
			}
			rfa.Done()

			//remove empty directories after fixing files
			dirLens := make(map[string]int)

			for dir := range recycleBinDirs {
				dirLens[dir] = len(dir)
			}

			sortedDirs := vangogh_integration.SortStrIntMap(dirLens, true)

			rda := nod.NewProgress(" removing leftover directories...")
			rda.TotalInt(len(sortedDirs))

			for _, dir := range sortedDirs {
				if err := os.Remove(dir); err != nil {
					return err
				}
				rda.Increment()
			}
			rda.Done()
		}
	}

	return nil
}
