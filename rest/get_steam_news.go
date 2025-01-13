package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetSteamNews(w http.ResponseWriter, r *http.Request) {

	// GET /steam-news?id

	id := r.URL.Query().Get("id")
	all := r.URL.Query().Has("all")

	sanReader, err := vangogh_integration.NewProductReader(vangogh_integration.SteamAppNews)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	san, err := sanReader.SteamGetAppNewsResponse(id)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.SteamNews(id, &san.AppNews, all)
	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
