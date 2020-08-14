package products

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/pages"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
)

func Fetch(client *http.Client, mediaType urls.MediaType, page int) (*ProductPage, error) {
	respBody, err := pages.Fetch(client, urls.ProductsPageURL(mediaType), page)
	if err != nil {
		return &ProductPage{}, err
	}

	var productPage ProductPage

	err = json.Unmarshal(*respBody, &productPage)
	return &productPage, err
}
