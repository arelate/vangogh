package products

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/pages"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
)

func Fetch(client *http.Client, mt media.Type, page int) (*ProductPage, error) {
	productPage := ProductPage{}

	respBody, err := pages.Fetch(client, urls.ProductsPageURL(mt), page)
	if err != nil {
		return &productPage, err
	}

	err = json.Unmarshal(*respBody, &productPage)
	return &productPage, err
}
