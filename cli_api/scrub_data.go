package cli_api

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/remove"
	"github.com/boggydigital/vangogh/cli_api/scrubs"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
	"os"
	"path/filepath"
)

func ScrubDataHandler(u *url.URL) error {
	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	fix := url_helpers.Flag(u, "fix")
	return ScrubData(mt, fix)
}

func ScrubData(mt gog_media.Media, fix bool) error {

	sda := nod.Begin("scrubbing local data for potential issues...")
	defer sda.End()

	if err := scrubLocalOnlySplitProducts(mt, fix); err != nil {
		return sda.EndWithError(err)
	}

	if err := scrubFilesInRecycleBin(fix); err != nil {
		return sda.EndWithError(err)
	}

	//products with values different from extracts
	//images that are not linked to a product
	//videos that are not linked to a product

	return nil
}

func scrubLocalOnlySplitProducts(mt gog_media.Media, fix bool) error {
	sloa := nod.Begin("checking for local only split products...")
	defer sloa.End()

	for _, pagedPt := range vangogh_products.Paged() {

		splitPt := vangogh_products.SplitType(pagedPt)

		pa := nod.Begin(" checking %s not present in %s...", splitPt, pagedPt)

		localOnlyProducts, err := scrubs.LocalOnlySplitProducts(pagedPt, mt)
		if err != nil {
			return pa.EndWithError(err)
		}

		if localOnlyProducts.Len() > 0 {
			pa.EndWithResult("found %d", localOnlyProducts.Len())
			if err := List(
				localOnlyProducts,
				0,
				splitPt,
				mt,
				nil); err != nil {
				return pa.EndWithError(err)
			}

			if fix {
				fa := nod.Begin(" removing local only %s...", splitPt)
				if err := remove.Data(localOnlyProducts.All(), splitPt, mt); err != nil {
					return fa.EndWithError(err)
				}
				fa.EndWithResult("done")
			}
		} else {
			pa.EndWithResult("%s and %s have all the same products", splitPt, pagedPt)
		}
	}

	sloa.EndWithResult("done")

	return nil
}

func scrubFilesInRecycleBin(fix bool) error {

	srba := nod.Begin(" checking files in recycle bin...")
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
		srba.EndWithResult("recycle bin is empty")
	} else {

		srba.EndWithResult("recycle bin contains %d file(s)", recycleBinFiles.Len())

		if fix {
			rfa := nod.NewProgress(" removing files in recycle bin...")
			rfa.TotalInt(recycleBinFiles.Len())
			for file := range recycleBinFiles {
				if err := os.Remove(filepath.Join(vangogh_urls.RecycleBinDir(), file)); err != nil {
					return rfa.EndWithError(err)
				}
				rfa.Increment()
			}
			rfa.EndWithResult("done")

			//remove empty directories after fixing files
			rda := nod.NewProgress(" removing directories in recycle bin...")
			rda.TotalInt(len(recycleBinDirs))
			for _, dir := range recycleBinDirs {
				if err := os.Remove(filepath.Join(vangogh_urls.RecycleBinDir(), dir)); err != nil {
					return rda.EndWithError(err)
				}
				rda.Increment()
			}
			rda.EndWithResult("done")
		}
	}

	return nil
}
