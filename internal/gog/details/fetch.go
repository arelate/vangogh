package details

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"io/ioutil"
	"net/http"
)

func Fetch(client *http.Client, id int, mt media.Type) (*Details, error) {

	resp, _ := client.Get(urls.Details(id, mt).String())
	defer resp.Body.Close()

	respBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return nil, err
	}

	var details Details

	err = json.Unmarshal(respBody, &details)
	if err != nil {
		return nil, err
	}

	return &details, nil
}
