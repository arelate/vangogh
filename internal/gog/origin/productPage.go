package origin

import (
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
)

type ProductPage struct {
	*Page
}

func NewProductPage(client *http.Client, mt media.Type) *ProductPage {
	return &ProductPage{
		&Page{
			Source: NewSource(client, urls.ProductsPageURL(mt)),
		},
	}
}
