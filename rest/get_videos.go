package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/arelate/vangogh_local_data"
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
	if vids, ok := rdx.GetAllValues(vangogh_local_data.VideoIdProperty, id); ok {
		videoIds = vids
	}
	slices.Sort(videoIds)

	videoTitles := make(map[string]string)
	videoDurations := make(map[string]string)

	for _, vid := range videoIds {

		if vtp, ok := rdx.GetLastVal(vangogh_local_data.VideoTitleProperty, vid); ok {
			videoTitles[vid] = vtp
		}
		if vdp, ok := rdx.GetLastVal(vangogh_local_data.VideoDurationProperty, vid); ok {
			videoDurations[vid] = vdp
		}
	}

	p := compton_pages.Videos(videoIds, videoTitles, videoDurations)
	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
