package fetch

import (
	"errors"
	"github.com/boggydigital/kevlar"
	"net/http"
	"net/url"
)

func SetValue(id string, u *url.URL, hc *http.Client, method string, authBearer string, kv kevlar.KeyValues) error {

	req, err := http.NewRequest(method, u.String(), nil)
	if err != nil {
		return err
	}

	if authBearer != "" {
		req.Header.Set("Authorization", "Bearer "+authBearer)
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
