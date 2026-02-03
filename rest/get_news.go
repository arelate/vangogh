package rest

import (
	"encoding/json"
	"net/http"

	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

func GetNews(w http.ResponseWriter, r *http.Request) {

	// GET /news?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	gogId := r.URL.Query().Get("id")
	all := r.URL.Query().Has("all")

	appNews, err := getAppNews(gogId)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.News(gogId, rdx, appNews, all)
	if err = p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
}

func getAppNews(gogId string) (*steam_integration.AppNews, error) {

	var steamAppId string
	if sai, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, gogId); ok && sai != "" {
		steamAppId = sai
	} else {
		return nil, nil
	}

	appNewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppNews)
	if err != nil {
		return nil, err
	}

	kvAppNews, err := kevlar.New(appNewsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	if !kvAppNews.Has(steamAppId) {
		return nil, nil
	}

	rcAppNews, err := kvAppNews.Get(steamAppId)
	if err != nil {
		return nil, err
	}
	defer rcAppNews.Close()

	var appNewsResponse steam_integration.GetNewsForAppResponse
	if err = json.NewDecoder(rcAppNews).Decode(&appNewsResponse); err != nil {
		return nil, err
	}

	return &appNewsResponse.AppNews, nil
}
