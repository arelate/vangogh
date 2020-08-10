package gamedetails

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"io/ioutil"
	"net/http"
)

func Fetch(client *http.Client, id int) (GameDetails, error) {

	resp, _ := client.Get(urls.GameDetails(id).String())
	defer resp.Body.Close()

	respBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return GameDetails{}, err
	}

	var gameDetails GameDetails

	err = json.Unmarshal(respBody, &gameDetails)
	if err != nil {
		return GameDetails{}, err
	}

	return gameDetails, nil
}
