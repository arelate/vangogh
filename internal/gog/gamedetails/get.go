package gamedetails

import (
	"github.com/boggydigital/vangogh/internal/gog/filenames"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"io/ioutil"
	"net/http"
)

func Get(client *http.Client, id int) {

	resp, _ := client.Get(urls.GameDetails(id).String())
	defer resp.Body.Close()

	body, _ := ioutil.ReadAll(resp.Body)

	ioutil.WriteFile(filenames.GameDetails(id), body, 0644)
}
