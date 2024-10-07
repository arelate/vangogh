package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/yet_urls/youtube_urls"
	"net/http"
	"net/url"
)

func GetVideoMetadataHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return GetVideoMetadata(
		idSet,
		vangogh_local_data.FlagFromUrl(u, "missing"),
		vangogh_local_data.FlagFromUrl(u, "force"))
}

func GetVideoMetadata(idSet map[string]bool, missing, force bool) error {

	gvma := nod.NewProgress("getting video metadata...")
	defer gvma.End()

	rdx, err := videoReduxAssets()
	if err != nil {
		return gvma.EndWithError(err)
	}

	videoIds := make([]string, 0, len(idSet))
	for id := range idSet {
		if vip, ok := rdx.GetAllValues(vangogh_local_data.VideoIdProperty, id); ok {
			for _, vid := range vip {
				if rdx.HasKey(vangogh_local_data.VideoTitleProperty, vid) && !force {
					continue
				}
				videoIds = append(videoIds, vid)
			}
		}
	}

	if missing {
		for _, id := range rdx.Keys(vangogh_local_data.VideoIdProperty) {
			if vip, ok := rdx.GetAllValues(vangogh_local_data.VideoIdProperty, id); ok {
				for _, vid := range vip {
					if !rdx.HasKey(vangogh_local_data.VideoTitleProperty, vid) {
						videoIds = append(videoIds, vid)
					}
				}
			}
		}
	}

	gvma.TotalInt(len(videoIds))
	videoTitles := make(map[string][]string)
	videoDurations := make(map[string][]string)

	for _, videoId := range videoIds {

		ipr, err := youtube_urls.GetVideoPage(http.DefaultClient, videoId)
		if err != nil {
			return gvma.EndWithError(err)
		}

		videoTitles[videoId] = []string{ipr.VideoDetails.Title}
		videoDurations[videoId] = []string{ipr.VideoDetails.LengthSeconds}

		gvma.Increment()
	}

	if err := rdx.BatchAddValues(vangogh_local_data.VideoTitleProperty, videoTitles); err != nil {
		return gvma.EndWithError(err)
	}

	if err := rdx.BatchAddValues(vangogh_local_data.VideoDurationProperty, videoDurations); err != nil {
		return gvma.EndWithError(err)
	}

	gvma.EndWithResult("done")

	return nil
}

func videoReduxAssets() (kevlar.WriteableRedux, error) {

	propSet := make(map[string]bool)
	propSet[vangogh_local_data.TitleProperty] = true

	for _, vp := range vangogh_local_data.VideoProperties() {
		propSet[vp] = true
	}

	properties := make([]string, len(propSet))
	for p := range propSet {
		properties = append(properties, p)
	}

	return vangogh_local_data.NewReduxWriter(properties...)
}
