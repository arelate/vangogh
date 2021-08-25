package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/remove"
	"os"
	"path/filepath"
	"strconv"
)

func ScrubData(mt gog_media.Media, fix bool) error {

	fmt.Println("split products missing from the paged data:")
	for _, pagedPt := range vangogh_products.Paged() {

		pagedIds := make(map[string]bool, 0)

		vrPaged, err := vangogh_values.NewReader(pagedPt, mt)
		if err != nil {
			return err
		}
		for _, id := range vrPaged.All() {
			productGetter, err := vrPaged.ProductsGetter(id)
			if err != nil {
				return err
			}
			for _, idGetter := range productGetter.GetProducts() {
				pagedIds[strconv.Itoa(idGetter.GetId())] = true
			}
		}

		splitIdSet := gost.NewStrSet()

		splitPt := vangogh_products.SplitType(pagedPt)
		vrSplit, err := vangogh_values.NewReader(splitPt, mt)
		if err != nil {
			return err
		}

		for _, id := range vrSplit.All() {
			if pagedIds[id] {
				continue
			}
			splitIdSet.Add(id)
		}

		if splitIdSet.Len() > 0 {
			fmt.Printf("%s not present in %s:\n", splitPt, pagedPt)
			if err := List(
				splitIdSet,
				0,
				splitPt,
				mt,
				nil); err != nil {
				return err
			}

			if fix {
				fmt.Printf("fix %s (%s):\n", splitPt, mt)
				if err := remove.Data(splitIdSet.All(), splitPt, mt); err != nil {
					return err
				}
			}
		} else {
			fmt.Printf("%s and %s have all the same products\n", splitPt, pagedPt)
		}
	}

	fmt.Println("files in recycle bin:")

	recycleBinFiles, err := vangogh_urls.RecycleBinFiles()
	if err != nil {
		return err
	}
	recycleBinDirs, err := vangogh_urls.RecycleBinDirs()
	if err != nil {
		return err
	}

	if recycleBinFiles.Len() == 0 && len(recycleBinDirs) == 0 {
		fmt.Println("recycle bin is empty")
	} else {
		for file := range recycleBinFiles {
			fmt.Print(file)
			if fix {
				if err := os.Remove(filepath.Join(vangogh_urls.RecycleBinDir(), file)); err != nil {
					return err
				}
				fmt.Print(" removed")
			}
			fmt.Println()
		}

		//remove empty directories after fixing files
		if fix {
			for _, dir := range recycleBinDirs {
				if err := os.Remove(filepath.Join(vangogh_urls.RecycleBinDir(), dir)); err != nil {
					return err
				}
				fmt.Printf("empty dir %s removed\n", dir)
			}
		}
	}

	// fmt.Println("products with values different from extracts:")
	// fmt.Println("images that are not linked to a product:")

	return nil
}
