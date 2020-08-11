package accountProducts

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/pages"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
)

func Fetch(client *http.Client, mediaType int, updated bool, hidden bool, page int) (*AccountProductPage, error) {
	respBody, err := pages.Fetch(client, urls.AccountProductsPageURL(mediaType, updated, hidden), page)
	if err != nil {
		return &AccountProductPage{}, err
	}

	var accountProductPage AccountProductPage

	err = json.Unmarshal(respBody, &accountProductPage)
	return &accountProductPage, err

}
