package wishlist

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/pages"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
)

func Fetch(client *http.Client, mt media.Type, hidden bool, page int) (*WishlistPage, error) {
	respBody, err := pages.Fetch(client, urls.WishlistPageURL(mt, hidden), page)
	if err != nil {
		return &WishlistPage{}, err
	}

	var wishlistPage WishlistPage

	err = json.Unmarshal(*respBody, &wishlistPage)
	return &wishlistPage, err

}
