package gog_data

import (
	"errors"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/kevlar"
	"net/http"
	"net/url"
)

func gogAuthHttpClient() (*http.Client, error) {
	acp, err := vangogh_integration.AbsCookiePath()
	if err != nil {
		return nil, err
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return nil, err
	}

	if err = gog_integration.IsLoggedIn(hc); err != nil {
		return nil, err
	}

	return hc, nil
}

func fetchGogData(id string, u *url.URL, hc *http.Client, method string, kv kevlar.KeyValues) error {

	req, err := http.NewRequest(method, u.String(), nil)
	if err != nil {
		return err
	}

	resp, err := hc.Do(req)
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return errors.New(id + ": " + resp.Status)
	}

	return kv.Set(id, resp.Body)
}
