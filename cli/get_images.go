package cli

import (
	"errors"
	"net/url"
	"os"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

func GetImagesHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return GetImages(
		ids,
		vangogh_integration.ImageTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "missing"),
		u.Query().Has("force"))
}

// GetImages fetches remote images for a given type (box-art, screenshots, background, etc.).
// If requested it can check locally present files and download all missing (used in data files,
// but not present locally) images for a given type.
func GetImages(ids []string, its []vangogh_integration.ImageType, missing, force bool) error {

	gia := nod.NewProgress("getting images...")
	defer gia.Done()

	rdx, err := imageTypesReduxAssets(nil, its)
	if err != nil {
		return err
	}

	//for every product we'll collect image types missing for id and download only those
	idMissingTypes := map[string][]vangogh_integration.ImageType{}

	if missing {
		localImageSet, err := vangogh_integration.LocalImageIds()
		if err != nil {
			return err
		}
		//to track image types missing for each product we do the following:
		//1. for every image type requested to be downloaded - get product ids that don't have this image type locally
		//2. for every product id we get this way - add this image type to idMissingTypes[id]
		for _, it := range its {
			//1
			missingImageIds, err := itemizations.MissingLocalImages(it, rdx, localImageSet)
			if err != nil {
				return err
			}

			//2
			for id := range missingImageIds {
				if idMissingTypes[id] == nil {
					idMissingTypes[id] = make([]vangogh_integration.ImageType, 0)
				}
				idMissingTypes[id] = append(idMissingTypes[id], it)
			}
		}
	} else {
		for _, id := range ids {
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

		title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
		if !ok {
			title = id
		}

		mita := nod.NewProgress("%s %s", id, title)
		missingImageTypes := map[vangogh_integration.ImageType]bool{}

		for _, it := range missingIts {

			images, ok := rdx.GetAllValues(vangogh_integration.PropertyFromImageType(it), id)
			if !ok || len(images) == 0 {
				nod.Log("%s missing %s", id, it)
				missingImageTypes[it] = true
				continue
			}

			srcUrls, err := vangogh_integration.ImagePropertyUrls(images, it)
			if err != nil {
				return err
			}

			for _, srcUrl := range srcUrls {
				if err = getImage(srcUrl, force); err != nil {
					nod.LogError(err)
				}
			}
		}

		completionStatus := "done"
		if len(missingImageTypes) > 0 {
			itss := make([]string, 0, len(missingImageTypes))
			for it := range missingImageTypes {
				itss = append(itss, it.String())
			}
			completionStatus = "no " + strings.Join(itss, ", ")
		}

		mita.EndWithResult(completionStatus)
		gia.Increment()
	}

	return nil
}

func imageTypesReduxAssets(otherProperties []string, its []vangogh_integration.ImageType) (redux.Writeable, error) {
	for _, it := range its {
		if !vangogh_integration.IsValidImageType(it) {
			return nil, errors.New("invalid image type: " + it.String())
		}
	}

	propSet := make(map[string]bool)
	propSet[vangogh_integration.TitleProperty] = true

	for _, it := range its {
		propSet[vangogh_integration.PropertyFromImageType(it)] = true
	}

	for _, p := range otherProperties {
		propSet[p] = true
	}

	properties := make([]string, 0, len(propSet))
	for p := range propSet {
		properties = append(properties, p)
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	return redux.NewWriter(reduxDir, properties...)
}

func getImage(imageUrl *url.URL, force bool) error {

	gia := nod.NewProgress(" %s...", imageUrl.Path)
	defer gia.Done()

	dstImageDir, err := vangogh_integration.AbsImagesDirByImageId(imageUrl.Path)
	if err != nil {
		return err
	}

	if _, err = os.Stat(dstImageDir); os.IsNotExist(err) {
		if err = os.MkdirAll(dstImageDir, 0755); err != nil {
			return err
		}
	}

	dc := reqs.GetDoloClient()

	return dc.Download(imageUrl, force, gia, dstImageDir)
}
