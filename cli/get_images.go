package cli

import (
	"fmt"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"net/url"
	"path/filepath"
	"strings"
)

func GetImagesHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return GetImages(
		idSet,
		vangogh_local_data.ImageTypesFromUrl(u),
		vangogh_local_data.FlagFromUrl(u, "missing"))
}

// GetImages fetches remote images for a given type (box-art, screenshots, background, etc.).
// If requested it can check locally present files and download all missing (used in data files,
// but not present locally) images for a given type.
func GetImages(
	idSet map[string]bool,
	its []vangogh_local_data.ImageType,
	missing bool) error {

	gia := nod.NewProgress("getting images...")
	defer gia.End()

	rdx, err := imageTypesReduxAssets(nil, its)
	if err != nil {
		return gia.EndWithError(err)
	}

	//for every product we'll collect image types missing for id and download only those
	idMissingTypes := map[string][]vangogh_local_data.ImageType{}

	if missing {
		localImageSet, err := vangogh_local_data.LocalImageIds()
		if err != nil {
			return gia.EndWithError(err)
		}
		//to track image types missing for each product we do the following:
		//1. for every image type requested to be downloaded - get product ids that don't have this image type locally
		//2. for every product id we get this way - add this image type to idMissingTypes[id]
		for _, it := range its {
			//1
			missingImageIds, err := itemizations.MissingLocalImages(it, rdx, localImageSet)
			if err != nil {
				return gia.EndWithError(err)
			}

			//2
			for id := range missingImageIds {
				if idMissingTypes[id] == nil {
					idMissingTypes[id] = make([]vangogh_local_data.ImageType, 0)
				}
				idMissingTypes[id] = append(idMissingTypes[id], it)
			}
		}
	} else {
		for id := range idSet {
			idMissingTypes[id] = its
		}
	}

	if len(idMissingTypes) == 0 {
		if missing {
			gia.EndWithResult("all images are available locally")
		} else {
			gia.EndWithResult("need at least one product id to get images")
		}
		return nil
	}

	gia.TotalInt(len(idMissingTypes))

	for id, missingIts := range idMissingTypes {

		//for every product collect all image URLs and all corresponding local filenames
		//to pass to dolo.GetSet, that'll concurrently download all required product images

		title, ok := rdx.GetFirstVal(vangogh_local_data.TitleProperty, id)
		if !ok {
			title = id
		}

		mita := nod.NewProgress("%s %s", id, title)
		missingImageTypes := map[vangogh_local_data.ImageType]bool{}

		//sensible assumption - we'll have at least as many URLs and filenames
		//as types of images we're missing
		urls := make([]*url.URL, 0, len(missingIts))
		filenames := make([]string, 0, len(missingIts))

		for _, it := range missingIts {

			images, ok := rdx.GetAllValues(vangogh_local_data.PropertyFromImageType(it), id)
			if !ok || len(images) == 0 {
				nod.Log("%s missing %s", id, it)
				missingImageTypes[it] = true
				continue
			}

			srcUrls, err := vangogh_local_data.ImagePropertyUrls(images, it)
			if err != nil {
				return mita.EndWithError(err)
			}

			urls = append(urls, srcUrls...)

			for _, srcUrl := range srcUrls {
				dstDir, err := vangogh_local_data.AbsImagesDirByImageId(srcUrl.Path)
				if err != nil {
					return mita.EndWithError(err)
				}
				filenames = append(filenames, filepath.Join(dstDir, srcUrl.Path))
			}
		}

		imagesIndexSetter := dolo.NewFileIndexSetter(filenames)

		//using http.DefaultClient as no image types require authentication
		//(this might change in the future)
		if errs := dolo.DefaultClient.GetSet(urls, imagesIndexSetter, mita); len(errs) > 0 {
			for ui, e := range errs {
				mita.Error(fmt.Errorf("GetSet %s error: %s", urls[ui], e.Error()))
			}
		}

		completionStatus := "done"
		if len(missingImageTypes) > 0 {
			itss := make([]string, 0, len(missingImageTypes))
			for it := range missingImageTypes {
				itss = append(itss, it.String())
			}
			completionStatus = fmt.Sprintf("no %s", strings.Join(itss, ", "))
		}

		mita.EndWithResult(completionStatus)
		gia.Increment()
	}

	gia.EndWithResult("done")

	return nil
}

func imageTypesReduxAssets(otherProperties []string, its []vangogh_local_data.ImageType) (kvas.WriteableRedux, error) {
	for _, it := range its {
		if !vangogh_local_data.IsValidImageType(it) {
			return nil, fmt.Errorf("invalid image type %s", it)
		}
	}

	propSet := make(map[string]bool)
	propSet[vangogh_local_data.TitleProperty] = true

	for _, it := range its {
		propSet[vangogh_local_data.PropertyFromImageType(it)] = true
	}

	for _, p := range otherProperties {
		propSet[p] = true
	}

	properties := make([]string, len(propSet))
	for p := range propSet {
		properties = append(properties, p)
	}

	return vangogh_local_data.NewReduxWriter(properties...)
}
