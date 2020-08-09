package auth

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"io/ioutil"
	"net/http"
)

func LoggedIn(client *http.Client) (bool, error) {

	resp, err := client.Get(urls.UserDataURL)

	if err != nil {
		return false, err
	}

	defer resp.Body.Close()

	respBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return false, err
	}

	var ud userData

	err = json.Unmarshal(respBody, &ud)
	if err != nil {
		return false, err
	}

	return ud.IsLoggedIn, nil
}
