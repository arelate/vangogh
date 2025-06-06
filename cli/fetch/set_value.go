package fetch

import (
	"errors"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"net/http"
	"net/url"
)

func RequestSetValue(id string, u *url.URL, itemReq *reqs.Params, kv kevlar.KeyValues) error {

	req, err := http.NewRequest(itemReq.HttpMethod, u.String(), nil)
	if err != nil {
		return err
	}

	if itemReq.UserAgent != "" {
		req.Header.Set("User-Agent", itemReq.UserAgent)
	}

	if itemReq.AuthBearer != "" {
		req.Header.Set("Authorization", "Bearer "+itemReq.AuthBearer)
	}

	resp, err := itemReq.HttpClient.Do(req)
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return errors.New(resp.Status)
	}

	return kv.Set(id, resp.Body)
}
