package gog_data

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
)

// TODO: Consider researching and adding Dreamlist games
// Open questions:
// - how to integrate them into vangogh? As a new top-level tab?
// - there's a lot of new data (estimate - 820 pages * 50 per page = 41 000 products), what's the primary use case?

func GetDreamlistPages(hc *http.Client, userAccessToken string) error {

	gdlpa := nod.NewProgress("getting %s...", vangogh_integration.DreamlistPage)
	defer gdlpa.Done()

	dreamlistPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.DreamlistPage)
	if err != nil {
		return err
	}

	kvDreamlistPages, err := kevlar.New(dreamlistPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = fetchGogPages(gog_integration.DreamlistPageUrl, hc, http.MethodGet, userAccessToken, kvDreamlistPages, gdlpa, true); err != nil {
		return err
	}

	return nil
}
