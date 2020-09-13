package remote

import (
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
)

type WishlistPage struct {
	*Page
}

func NewWishlistPage(client *http.Client, mt media.Type) *WishlistPage {
	return &WishlistPage{
		&Page{
			Source: NewSource(client, urls.WishlistPageURL(mt, false)),
		},
	}
}
