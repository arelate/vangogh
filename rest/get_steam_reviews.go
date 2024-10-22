package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetSteamReviews(w http.ResponseWriter, r *http.Request) {

	// GET /steam-reviews?id

	id := r.URL.Query().Get("id")

	sarReader, err := vangogh_local_data.NewProductReader(vangogh_local_data.SteamReviews)

	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	sar, err := sarReader.SteamAppReviews(id)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.SteamReviews(id, sar)

	if err := p.Write(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
