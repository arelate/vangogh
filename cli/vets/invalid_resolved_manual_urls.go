package vets

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"path"
)

func InvalidResolvedManualUrls(fix bool) error {

	cirmu := nod.NewProgress("checking invalid resolved manual-urls...")
	defer cirmu.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.LocalManualUrlProperty)
	if err != nil {
		return cirmu.EndWithError(err)
	}

	invalidResolvedUrls := make(map[string]bool)

	keys := rxa.Keys(vangogh_local_data.LocalManualUrlProperty)
	cirmu.TotalInt(len(keys))
	for _, url := range keys {

		local, ok := rxa.GetFirstVal(vangogh_local_data.LocalManualUrlProperty, url)
		if !ok {
			continue
		}

		_, urlFile := path.Split(url)
		_, localFile := path.Split(local)

		if urlFile != localFile {
			continue
		}

		invalidResolvedUrls[url] = true

		cirmu.Increment()
	}

	if len(invalidResolvedUrls) == 0 {
		cirmu.EndWithResult("all good")
	} else {

		summary := make(map[string][]string)

		var firmu nod.TotalProgressWriter
		if fix {
			firmu = nod.NewProgress("fixing invalid resolved manual-urls...")
			firmu.TotalInt(len(invalidResolvedUrls))
		}

		for url := range invalidResolvedUrls {
			local, _ := rxa.GetFirstVal(vangogh_local_data.LocalManualUrlProperty, url)
			summary[url] = []string{local}
			if fix {
				// remove the entry from the redux
				if err := rxa.CutVal(vangogh_local_data.LocalManualUrlProperty, url, local); err != nil {
					cirmu.Error(err)
				}

				// move local file to the recycle bin
				absLocalFilepath := vangogh_local_data.AbsDownloadDirFromRel(local)
				if err := vangogh_local_data.MoveToRecycleBin(absLocalFilepath); err != nil {
					cirmu.Error(err)
				}
				if firmu != nil {
					firmu.Increment()
				}
			}
		}

		format := "found %d problems:"
		if fix {
			firmu.EndWithResult("done")
			format = "found and fixed %d problems:"
		}

		heading := fmt.Sprintf(format, len(invalidResolvedUrls))

		if err != nil {
			return cirmu.EndWithError(err)
		}
		cirmu.EndWithSummary(heading, summary)
	}

	return nil
}
