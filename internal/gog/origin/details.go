package origin

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/dest"
	"net/http"
	"net/url"
	"strconv"
	"strings"
)

const idPlaceholder = "{id}"

type Details struct {
	*Source
}

func NewDetails(httpClient *http.Client, urlTemplate *url.URL) *Details {
	return &Details{
		Source: NewSource(httpClient, urlTemplate),
	}
}

func (detailsSource *Details) Get(id int) (*[]byte, error) {
	// use id
	detailsSource.URL.Path = strings.Replace(detailsSource.URL.Path, idPlaceholder, strconv.Itoa(id), 1)

	return detailsSource.Source.Get(id)
}

func (detailsSource *Details) Transfer(id int, setter dest.Setter) error {
	bytes, _ := detailsSource.Get(id)

	var det map[string]interface{}
	_ = json.Unmarshal(*bytes, &det)
	det["id"] = id

	djson, _ := json.Marshal(det)
	_ = setter.Set(djson)

	return nil

}
