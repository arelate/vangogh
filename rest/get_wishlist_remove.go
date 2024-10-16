package rest

import (
	"github.com/arelate/vangogh/paths"
	"github.com/arelate/vangogh_local_data"
	"net/http"
)

func GetWishlistRemove(w http.ResponseWriter, r *http.Request) {

	// GET /wishlist/remove?id

	id := r.URL.Query().Get(vangogh_local_data.IdProperty)

	//TODO: Restore from vangogh
	//if err := deleteWishlist(http.DefaultClient, id); err != nil {
	//	http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	//	return
	//}

	http.Redirect(w, r, paths.ProductId(id), http.StatusTemporaryRedirect)
}
