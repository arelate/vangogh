package cli_api

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
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

	if err := scrubInvalidLocalProductData(mt, fix); err != nil {
		return sda.EndWithError(err)
	}

	//products with values different from extracts
	//images that are not linked to a product
	//videos that are not linked to a product
	//logs older than 30 days

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
			pa.EndWithResult("none found")
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
			sortDirs := gost.NewSortStrSet()
			dirLens := make(map[string]int)

			for _, dir := range recycleBinDirs.All() {
				sortDirs.Add(dir)
				dirLens[dir] = len(dir)
			}

			sortedDirs := sortDirs.SortByIntVal(dirLens, true)

			rda := nod.NewProgress(" removing directories in recycle bin...")
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

func scrubInvalidLocalProductData(mt gog_media.Media, fix bool) error {
	ilpa := nod.NewProgress(" checking data for invalid products...")
	defer ilpa.End()

	invalidProducts := make(map[vangogh_products.ProductType][]string)

	allProductTypes := make(map[vangogh_products.ProductType]bool)
	for _, pt := range append(vangogh_products.Remote(), vangogh_products.Local()...) {
		allProductTypes[pt] = true
	}

	ilpa.TotalInt(len(allProductTypes))

	dataProblems := false

	for pt := range allProductTypes {

		if pt == vangogh_products.LicenceProducts {
			continue
		}

		invalidProducts[pt] = make([]string, 0)

		pta := nod.NewProgress(" checking %s...", pt)

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			_ = pta.EndWithError(err)
			continue
		}

		allProducts := vr.All()

		pta.TotalInt(len(allProducts))

		for _, id := range allProducts {
			prd, err := vr.ReadValue(id)
			if err != nil || prd == nil {
				invalidProducts[pt] = append(invalidProducts[pt], id)
				dataProblems = true
				if fix {
					if err := vr.Remove(id); err != nil {
						return err
					}
				}
			}
			pta.Increment()
		}

		pta.EndWithResult("done")
	}

	if !dataProblems {
		ilpa.EndWithResult("data seems ok")
	} else {
		exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty)
		if err != nil {
			return err
		}
		summary := make(map[string][]string)
		for pt, ids := range invalidProducts {
			if len(ids) == 0 {
				continue
			}
			ptStr := fmt.Sprintf("problems with %s:", pt)
			summary[ptStr] = make([]string, len(ids))
			for i := 0; i < len(ids); i++ {
				prodStr := ids[i]
				if title, ok := exl.Get(vangogh_properties.TitleProperty, ids[i]); ok {
					prodStr = fmt.Sprintf("%s %s", prodStr, title)
				}
				summary[ptStr][i] = prodStr
			}
		}
		ilpa.EndWithSummary(summary)
	}

	return nil
}
