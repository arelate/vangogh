package rest

import (
	"net/http"
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetGogWishlist(w http.ResponseWriter, r *http.Request) {

	// GET /gog/wishlist

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := make(url.Values)
	q.Set(vangogh_integration.GogUserWishlistProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.UrlSortParameter, vangogh_integration.GogReleaseDateProperty)
	q.Set(vangogh_integration.UrlDescendingParameter, vangogh_integration.TrueValue)

	ids, from, to, err := searchResults(q)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	gogSectionPage := compton_pages.GogSearch("Wishlist", q, ids, from, to, rdx)
	if err = gogSectionPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}
