package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"net/http"
	"slices"
)

func GetVideos(w http.ResponseWriter, r *http.Request) {

	// GET /videos?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	var videoIds []string
	if vids, ok := rdx.GetAllValues(vangogh_integration.VideoIdProperty, id); ok {
		videoIds = vids
	}
	slices.Sort(videoIds)

	videoTitles := make(map[string]string)
	videoDurations := make(map[string]string)

	for _, vid := range videoIds {

		if vtp, ok := rdx.GetLastVal(vangogh_integration.VideoTitleProperty, vid); ok {
			videoTitles[vid] = vtp
		}
		if vdp, ok := rdx.GetLastVal(vangogh_integration.VideoDurationProperty, vid); ok {
			videoDurations[vid] = vdp
		}
	}

	p := compton_pages.Videos(id, videoIds, videoTitles, videoDurations, rdx)
	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
