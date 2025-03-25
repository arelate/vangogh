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

func GetRatingsReviews(w http.ResponseWriter, r *http.Request) {

	// GET /ratings-reviews?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	gogId := r.URL.Query().Get("id")

	appReviews, err := getSteamReviews(gogId)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.RatingsReviews(gogId, appReviews, rdx)

	if err := p.Write(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}

func getSteamReviews(gogId string) (*steam_integration.AppReviews, error) {

	var steamAppId string
	if sai, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, gogId); ok && sai != "" {
		steamAppId = sai
	} else {
		return nil, errors.New("no steam app id for gog id " + gogId)
	}

	appReviewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppReviews)
	if err != nil {
		return nil, err
	}

	kvAppReviews, err := kevlar.New(appReviewsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	rcAppReviews, err := kvAppReviews.Get(steamAppId)
	if err != nil {
		return nil, err
	}
	defer rcAppReviews.Close()

	var appReviews steam_integration.AppReviews
	if err = json.NewDecoder(rcAppReviews).Decode(&appReviews); err != nil {
		return nil, err
	}

	return &appReviews, nil

}
