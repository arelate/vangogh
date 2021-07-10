package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
)

func Size(ids []string,
	slug string,
	mt gog_media.Media,
	osStrings []string,
	langCodes []string,
	dtStrings []string,
	missing bool) error {

	dlList := vangogh_downloads.DownloadsList{}

	if err := getDownloadsList(
		ids,
		slug,
		mt,
		osStrings,
		langCodes,
		dtStrings,
		missing,
		func(list vangogh_downloads.DownloadsList) {
			dlList = append(dlList, list...)
		}); err != nil {
		return err
	}

	fmt.Printf("estimated total download size: %.2fGB\n", dlList.TotalGBsEstimate())

	return nil
}
