package vets

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"path"
)

func InvalidResolvedManualUrls(fix bool) error {

	cirmu := nod.NewProgress("checking invalid resolved manual-urls...")
	defer cirmu.End()

	rdx, err := vangogh_integration.NewReduxWriter(
		vangogh_integration.LocalManualUrlProperty)
	if err != nil {
		return cirmu.EndWithError(err)
	}

	invalidResolvedUrls := make(map[string]bool)

	keys := rdx.Keys(vangogh_integration.LocalManualUrlProperty)
	cirmu.TotalInt(len(keys))
	for _, url := range keys {

		local, ok := rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, url)
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

		adp, err := pathways.GetAbsDir(vangogh_integration.Downloads)
		if err != nil {
			return cirmu.EndWithError(err)
		}

		for url := range invalidResolvedUrls {
			local, _ := rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, url)
			summary[url] = []string{local}
			if fix {
				// remove the entry from the redux
				if err := rdx.CutValues(vangogh_integration.LocalManualUrlProperty, url, local); err != nil {
					cirmu.Error(err)
				}

				// move local file to the recycle bin
				absLocalFilepath, err := vangogh_integration.AbsDownloadDirFromRel(local)
				if err != nil {
					cirmu.Error(err)
				}
				if err := vangogh_integration.MoveToRecycleBin(adp, absLocalFilepath); err != nil {
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
