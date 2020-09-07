package origin

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/dest"
	"net/http"
	"net/url"
	"strconv"
)

const pageKey = "page"

type Page struct {
	*Source
}

func NewPage(client *http.Client, url *url.URL) *Page {
	return &Page{
		Source: NewSource(client, url),
	}
}

func (pageSource *Page) Get(page int) (*[]byte, error) {

	q := pageSource.URL.Query()
	q.Add(pageKey, strconv.Itoa(page))
	pageSource.URL.RawQuery = q.Encode()

	return pageSource.Source.Get(page)
}

func (pageSource *Page) FetchPage(page int, setter dest.Setter) (totalPages int, err error) {

	bytes, _ := pageSource.Get(page)

	var pageData map[string]interface{}
	_ = json.Unmarshal(*bytes, &pageData)

	for _, p := range pageData["products"].([]interface{}) {
		pjson, _ := json.Marshal(p)
		_ = setter.Set(pjson)
	}

	return int(pageData["totalPages"].(float64)), nil
}
