package iterate

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
)

type iterateDownloadListDelegate func(
	id string,
	slug string,
	dlList vangogh_downloads.DownloadsList,
	exl *vangogh_extracts.ExtractsList,
	forceRemoteUpdate bool) error

func DownloadsList(
	idSet gost.StrSet,
	mt gog_media.Media,
	exl *vangogh_extracts.ExtractsList,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	delegate iterateDownloadListDelegate,
	modifiedSince int64,
	forceRemoteUpdate bool) error {

	if delegate == nil {
		return fmt.Errorf("vangogh: get downloads list delegate is nil")
	}
	if err := exl.AssertSupport(
		vangogh_properties.SlugProperty,
		vangogh_properties.NativeLanguageNameProperty); err != nil {
		return err
	}

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	if err != nil {
		return err
	}

	vrAccountProducts, err := vangogh_values.NewReader(vangogh_products.AccountProducts, mt)

	for _, id := range idSet.All() {

		detSlug, ok := exl.Get(vangogh_properties.SlugProperty, id)

		if !vrDetails.Contains(id) || !ok {
			continue
		}

		det, err := vrDetails.Details(id)
		if err != nil {
			return err
		}

		downloads, err := vangogh_downloads.FromDetails(det, mt, exl)
		if err != nil {
			return err
		}

		if !forceRemoteUpdate {
			forceRemoteUpdate = modifiedSince > 0 &&
				(vrDetails.WasModifiedAfter(id, modifiedSince) ||
					vrAccountProducts.WasModifiedAfter(id, modifiedSince))
		}

		filteredDownloads := make([]vangogh_downloads.Download, 0)

		for _, dl := range downloads.Only(operatingSystems, downloadTypes, langCodes) {
			//some manualUrls have "0 MB" specified as size and don't seem to be used to create user clickable links.
			//resolving such manualUrls leads to an empty filename
			//given they don't contribute anything to download, size or validate commands - we're filtering them
			if dl.EstimatedBytes == 0 {
				continue
			}
			filteredDownloads = append(filteredDownloads, dl)

		}

		// already checked for nil earlier in the function
		if err := delegate(
			id,
			detSlug,
			filteredDownloads,
			exl,
			forceRemoteUpdate); err != nil {
			return err
		}
	}

	return nil
}
