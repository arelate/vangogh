package rest

import (
	"net/http"
	"slices"

	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/nod"
)

func GetGogDispatch(w http.ResponseWriter, r *http.Request) {

	// GET /gog/dispatch

	permissions, err := sb.GetCookiePermissions(r)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if slices.Contains(permissions, perm.ReadOwned) {
		http.Redirect(w, r, "/gog/owned", http.StatusPermanentRedirect)
		return
	} else if slices.Contains(permissions, perm.ReadProductData) {
		http.Redirect(w, r, "/gog/catalog", http.StatusPermanentRedirect)
		return
	}
}
