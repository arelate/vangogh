package gamedetails

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"io/ioutil"
	"net/http"
)

func Get(client *http.Client, id int) (GameDetails, error) {

	resp, _ := client.Get(urls.GameDetails(id).String())
	defer resp.Body.Close()

	respBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return GameDetails{}, err
	}

	var gd GameDetails

	err = json.Unmarshal(respBody, &gd)
	if err != nil {
		return GameDetails{}, err
	}

	return gd, nil
}
