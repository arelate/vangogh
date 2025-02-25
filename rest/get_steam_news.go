package rest

import (
	"encoding/json"
	"errors"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetSteamNews(w http.ResponseWriter, r *http.Request) {

	// GET /steam-news?id

	gogId := r.URL.Query().Get("id")
	all := r.URL.Query().Has("all")

	appNews, err := getAppNews(gogId)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.SteamNews(gogId, appNews, all)
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
		return nil, errors.New("no steam app id for gog id " + gogId)
	}

	appNewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppNews)
	if err != nil {
		return nil, err
	}

	kvAppNews, err := kevlar.New(appNewsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
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
